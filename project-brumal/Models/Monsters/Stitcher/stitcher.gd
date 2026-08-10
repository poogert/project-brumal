extends Node3D
@onready var animation_player: AnimationPlayer = $AnimationPlayer
@onready var animation_tree: AnimationTree = $AnimationTree
@onready var area_3d: Area3D = $Area3D

@onready var timer: Timer = $Timer

var check1 = true
# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	if (area_3d.overlaps_body($"../Player") && check1 == true):
		check1 = false
		print("contact")
		timer.start()
		animation_tree.set("parameters/StateMachine/conditions/Area3D", true)
		
						
		
	


func _on_timer_timeout() -> void:
	#if(animation_player.is_playing()):
		self.queue_free()
