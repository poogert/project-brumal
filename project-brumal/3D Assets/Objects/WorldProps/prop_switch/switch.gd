class_name Switch
extends Node3D

var switch_toggle: bool = false
var switch_handle: Node3D
var brass_mesh: MeshInstance3D

var off := Vector3.ZERO
var on := Vector3(90, 0, 0)

var tween: Tween

var target_rotation: Vector3 = Vector3(90, 0, 0)

signal switch_status(status: bool)

func _ready() -> void:
	switch_handle = get_node("SwitchHandle") as Node3D
	brass_mesh = get_node("SwitchHousingBrass/BrassHouseMesh") as MeshInstance3D

func interact() -> void:
	
	switch_toggle = not switch_toggle
	switch_status.emit(switch_toggle)
	
	target_rotation = on if switch_toggle else off
	
	if tween:
		tween.kill()
	
	target_rotation = on if switch_toggle else off
	
	tween = create_tween()
	
	tween.set_trans(Tween.TRANS_SPRING)
	tween.set_ease(Tween.EASE_IN)
	
	tween.tween_property(switch_handle, "rotation_degrees", target_rotation, 0.2)
