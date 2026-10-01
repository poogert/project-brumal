extends Node3D

var omnilight: OmniLight3D
@export var _switch: Switch

func _ready() -> void:
	_switch.switch_status.connect(_on_switch_status_reached_signal)
	omnilight = get_node("LightBulb/Bulb") as OmniLight3D
	print( str(omnilight) + " <--- omnilight")


func _on_switch_status_reached_signal(switch_toggle: bool) -> void:
	print("signalcalled")
	if switch_toggle:
		omnilight.light_energy = 1
	else:
		omnilight.light_energy = 0
	
