module Graphic

open Raylib_cs

let toBool (x: CBool) : bool = x = CBool.op_Implicit true

type Input = { 
    Left: bool
    Right: bool
    Up: bool
    Down: bool 
}

type Rectangle = { 
    X: single
    Y: single
    W: int
    H: int
    Color: Color 
}

type GameState = { 
    Input: Input 
    Rectangles: Rectangle[]
}

let drawRectangles (rectangles: Rectangle[]) =
    let rec loop (i: int) =
        if i >= rectangles.Length 
            then ()
        else 
            Raylib.DrawRectangle (int rectangles[i].X, int rectangles[i].Y, rectangles[i].W, rectangles[i].H, rectangles[i].Color)
            loop (i + 1)
    
    Raylib.BeginDrawing()
    Raylib.ClearBackground(Color(38uy, 46uy, 56uy, 255uy)) 
    loop 0
    Raylib.DrawText("ВОва лох", 10, 10, 20, Color.White)
    Raylib.EndDrawing()

