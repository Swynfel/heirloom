tool
extends RichTextEffect

# Syntax: [rem data=rememberId][/rem]

var bbcode = "rem"
var color = Color(0.2, 0.75, 0.95)

func _process_custom_fx(char_fx):
    char_fx.color = color
    return true
