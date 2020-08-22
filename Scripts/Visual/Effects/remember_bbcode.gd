tool
extends RichTextEffect

# Syntax: [rem data=rememberId][/rem]

var bbcode = "rem"
var color = Color(0.5, 0.8, 1)

func _process_custom_fx(char_fx):
    char_fx.color = color
    return true
