shader_type canvas_item;

const vec4 BLACK = vec4(0,0,0,1);
uniform vec4 outline : hint_color;

void fragment() {
    COLOR = texture(TEXTURE,UV);
    if(COLOR == BLACK) {
        COLOR = outline;
    }
}