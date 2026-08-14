extends Container


# Called when the node enters the scene tree for the first time.
func _ready() -> void:
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta: float) -> void:
	## remember ctrl + k to toggle comment on highlighted text - E
	#if get_tree().current_scene.name == "Player":
		#print(PlayerData.CurrentState())
		#if(PlayerData.CurrentState() == "idle"):
			#$StandSprite.visible = true
		#else:
			#$StandSprite.visible = false
		#if(PlayerData.CurrentState() == "crouching"):
			#$CrouchSprite.visible = true
		#else:
			#$CrouchSprite.visible = false
		#if(PlayerData.CurrentState() == "crawling"):
			#$CrawlSprite.visible = true
		#else:
			#$CrawlSprite.visible = false
	##else: 
		##print("fail")
		pass
