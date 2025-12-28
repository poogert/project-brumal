extends MarginContainer

@export var menu_screen: VBoxContainer
@export var buttonPic: Button
@export var buttonFlare: Button
@export var buttonLight: Button
@export var buttonGrapple: Button
var buttonArray: Array[Button] = []

func _ready():
	buttonArray = [buttonPic,buttonFlare,buttonLight,buttonGrapple]
	for button in buttonArray:
		button.disabled = true

func enable_only(target: Button):
	for button in buttonArray:
		if button:
			button.disabled = (button != target)
			#print(Input,"changed",buttonArray, "Scale but not any other")

func _process(_delta):
	if Input.is_action_just_pressed("itemone") and buttonPic:
		enable_only(buttonPic)

	if Input.is_action_just_pressed("itemtwo") and buttonFlare:
		enable_only(buttonFlare)

	if Input.is_action_just_pressed("itemthree") and buttonLight:
		enable_only(buttonLight)

	if Input.is_action_just_pressed("itemfour") and buttonGrapple:
		enable_only(buttonGrapple)

	if Input.is_action_just_pressed("toggle_menu"):
		menu_screen.visible = !menu_screen.visible
		#print(menu_screen.visible)
		
