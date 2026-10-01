extends Node3D

var _light: SpotLight3D

## timer that determines how much fuel is consumed
@export var _fuel_timer: Timer

@export var default_spot_angle: float = 60 
@export var default_spot_range: float = 50
@export var default_brightness: float = 8.0

@export var flicker_intensity: float = 0.3
@export var flicker_threshold: float = 0.6

var lamp_active: bool = false
var flashing_active: bool = false
var flicker_enabled: bool = true

@onready var player_data: Node = get_node("/root/PlayerData")

func _ready() -> void:

	_light = get_node("light") as SpotLight3D
	_fuel_timer.timeout.connect(_on_fuel_timer_timeout)

	turn_light_off()

## frame based input/flash handling
func _process(delta: float) -> void:
	handle_input()
	update_flicker()


func handle_input():
	
	if ( Input.is_action_just_pressed("leftclick") ):

		if ( lamp_active ):
			turn_light_off()
		else: 
			turn_light_on()
	
	if ( Input.is_action_just_pressed("special") ):
		start_flash()


func turn_light_on() -> void:
	
	# no light if out of fuel
	if (lamp_active || get_calcium_fuel() <= 0): 
		return

	lamp_active 			= true
	_light.light_energy = default_brightness
	_light.spot_angle	= default_spot_angle
	_light.spot_range	= default_spot_range
	_fuel_timer.start()


func turn_light_off() -> void:
	
	if (not lamp_active):
		return

	lamp_active			= false

	_fuel_timer.stop()
	_light.light_energy = 0

## a number dictates whether flicker should occur
## if it does a random flicker amount happens
func calculate_flicker() -> float:

	var ran_a: float = randfn(0.0, 1.0)
	var ran_b: float = randfn(0.0, 1.0)

	var should_flicker: bool = ran_a > flicker_threshold
	var random_flicker_amount: float = 2 + (ran_b * flicker_intensity)

	var output: float = (
		random_flicker_amount if 
		should_flicker else default_brightness
	)

	return output

	
	
func start_flash() -> void:
	if (flashing_active) or (get_calcium_fuel() <= 20.0):
		return
	
	flashing_active = true
	flicker_enabled = false

	turn_light_on()
	set_calcium_fuel(get_calcium_fuel() - 20.0);

	_light.light_energy = 3000
	_light.spot_angle = 179.0
	_light.spot_range = 200.0
	update_flash()
	

## tween based instead of lerp
func update_flash() -> void:

	if not flashing_active:
		return


	var tween := create_tween().set_parallel(true)

	# exponential pop,
	tween.set_trans(Tween.TRANS_EXPO)
	tween.set_ease(Tween.EASE_OUT)

	# tween.tween_property (object, property, starting point, timespan)
	tween.tween_property(_light, "light_energy", (default_brightness - 1), 1.0)
	tween.tween_property(_light, "spot_angle", default_spot_angle, 1.0)
	tween.tween_property(_light, "spot_range", default_spot_range, 1.0)

	await tween.finished
	end_flash()

func end_flash() -> void:

	_light.light_energy = (
		default_brightness if 
		lamp_active
		else 0.0
	)
	_light.spot_angle = default_spot_angle
	_light.spot_range = default_spot_range

	flicker_enabled = true
	flashing_active = false


func update_flicker() -> void:
	
	if flashing_active:
		return
	
	if not lamp_active:
		_light.light_energy = 0;
		return
	
	if flicker_enabled:
		_light.light_energy = calculate_flicker()
	else:
		_light.light_energy = default_brightness
	

func _on_fuel_timer_timeout() -> void:
	if not lamp_active:
		return

	set_calcium_fuel(
		max(0, (get_calcium_fuel() - 1))
	)

	if get_calcium_fuel() <= 0:
		turn_light_off()



# ========================================================================
# these are temporary since player data will be replaced
# ========================================================================

func get_calcium_fuel() -> float:
	return player_data.GetCalciumFuel()

func set_calcium_fuel(value: float) -> void:
	player_data.SetCalciumFuel(value)
