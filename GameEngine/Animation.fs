module Animation
open Raylib_cs
open Rectangle
open System.Collections.Generic
open System.Numerics
open System.IO

type Point = {
    X: int
    Y: int
}

type Camera = {
    X: single
    Y: single
    W: int 
    H: int
    Speed: single
    initialSpeed: single
}

type Color = {
    R: byte
    G: byte 
    B: byte
    Alpha: byte
}

type Animation = {
    Name: string
    Frames: Texture2D[]
    CurrentFrame: int
    FrameCounter: int
    FrameSpeed: int
}

type DrawableObject = {
    Point: Point
    W: int
    H: int
    Color: Color
    Animations: Map<string, Animation>
    CurrentAnimationName: string
}

type GraphicObject =
| DrawableObject of DrawableObject
| RelativeDrawableObject of {|Object: GraphicObject; RelatePoint: Point|}
| MaskedGraphicObject of {|Object: GraphicObject; Mask: Texture2D|}
| Scope of {|Objects: GraphicObject[]; Point: Point; W: int; H: int|}

let private toRaylibColor (color: Color) = 
    Color(color.R, color.G, color.B, color.Alpha)

let addPoint (point1: Point) (point2: Point) =
    { X = point1.X + point2.X; Y = point1.Y + point2.Y }

let rec getSizesOfGraphicObject (graphicObject: GraphicObject) = 
    match graphicObject with 
        | DrawableObject obj -> obj.W, obj.H
        | RelativeDrawableObject obj -> getSizesOfGraphicObject obj.Object
        | Scope scope -> scope.W, scope.H
        | MaskedGraphicObject masked -> getSizesOfGraphicObject masked.Object

let rec getPointOfGraphicObject (graphicObject: GraphicObject) = 
    match graphicObject with 
        | DrawableObject obj -> obj.Point
        | RelativeDrawableObject obj -> obj.RelatePoint
        | Scope scope -> scope.Point
        | MaskedGraphicObject masked -> getPointOfGraphicObject masked.Object

let rec addPointToGraphicObjectPoint (graphicObject: GraphicObject) (point: Point) = 
    match graphicObject with 
        | DrawableObject obj -> DrawableObject {obj with Point = addPoint obj.Point point}
        | RelativeDrawableObject obj -> RelativeDrawableObject {|obj with RelatePoint = addPoint obj.RelatePoint point|}
        | Scope scope -> Scope {|scope with Point = addPoint scope.Point point|}
        | MaskedGraphicObject masked -> MaskedGraphicObject {|masked with Object = addPointToGraphicObjectPoint masked.Object point|}

let drawTexture (texture: Texture2D) (width: int) (height: int) (point: Point) (color: Color)=
    let sourceRec = Rectangle(0.0f, 0.0f, float32 texture.Width, float32 texture.Height)
    let destRec = Rectangle(float32 point.X, float32 point.Y, float32 width, float32 height)
    Raylib.DrawTexturePro(texture, sourceRec, destRec, Vector2.Zero, 0.0f, toRaylibColor color)

let drawDrawableObject (object: DrawableObject) =
    let currentAnimation = object.Animations.[object.CurrentAnimationName]
    let currentTexture = currentAnimation.Frames.[currentAnimation.CurrentFrame]
    drawTexture currentTexture object.W object.H object.Point object.Color

let drawMask (masked: {|Object: GraphicObject; Mask: Texture2D|}) =
    let W, H = getSizesOfGraphicObject masked.Object
    let whiteColor = {R = 255uy; G = 255uy; B = 255uy; Alpha = 255uy}
    drawTexture masked.Mask W H (getPointOfGraphicObject masked.Object) whiteColor

let rec drawGraphicObject (graphicObject: GraphicObject) =
    match graphicObject with 
    | DrawableObject obj -> drawDrawableObject obj 
    | RelativeDrawableObject obj -> drawGraphicObject (addPointToGraphicObjectPoint obj.Object obj.RelatePoint)
    | MaskedGraphicObject obj -> 
        drawGraphicObject obj.Object
        drawMask obj 
    | Scope scope -> scope.Objects 
                    |> Array.map (fun obj -> addPointToGraphicObjectPoint obj scope.Point) 
                    |> Array.iter drawGraphicObject


let createMask (object: GraphicObject) (colorCalculator: Point -> Color) =
    let width, height = getSizesOfGraphicObject object
        
    let mutable image = Raylib.GenImageColor(width, height, Color(0, 0, 0, 255))
    
    for y in 0..height-1 do
        for x in 0..width-1 do
            let color = toRaylibColor (colorCalculator {X = x; Y = y})
            Raylib.ImageDrawPixel(&image, x, y, color)
    
    Raylib.LoadTextureFromImage image

let loadAnimation (framePaths: string[]) frameSpeed =
    let frames = framePaths |> Array.map Raylib.LoadTexture
    {
        Name = "Attack"
        Frames = frames
        CurrentFrame = 0
        FrameCounter = 0
        FrameSpeed = frameSpeed
    }

let updateAnimation (animation: Animation) =
    if animation.Frames.Length > 1 then
        let newCounter = animation.FrameCounter + 1
        if newCounter >= animation.FrameSpeed then
            let nextFrame = (animation.CurrentFrame + 1) % animation.Frames.Length
            { animation with 
                CurrentFrame = nextFrame
                FrameCounter = 0 }
        else
            { animation with FrameCounter = newCounter }
    else
        animation


let isVisible (graphicObject: GraphicObject) (camera: Camera) =
    let { X = objectX; Y = objectY } = getPointOfGraphicObject graphicObject
    let inCameraX = int (objectX - camera.X) + (camera.W / 2 - objectWidth / 2)
    let inCameraY = int (objectY - camera.Y) + (camera.H / 2 - objectHeigth / 2)
    0 <= inCameraX + objectWidth && inCameraX - objectWidth <= camera.W && 0 <= inCameraY + objectHeigth && inCameraY - objectHeigth <= camera.H

let drawRectangles (rectangles: Rectangle[]) (camera: Camera) =
    let drawOneRectangle (rectangle: Rectangle) =
        
        if isVisible rectangle camera 
            then Raylib.DrawRectangle (inCameraX, inCameraY, rectangle.W, rectangle.H, rectangle.Color)
    
    let rec loop (i: int) =
        if i >= rectangles.Length 
            then ()
        else 
            drawOneRectangle rectangles.[i]
            loop (i + 1)
    
    Raylib.BeginDrawing()
    Raylib.ClearBackground(Color(38uy, 46uy, 56uy, 255uy)) 
    loop 0
    Raylib.DrawText("ВОва лох", 10, 10, 20, Color.White)
    Raylib.EndDrawing()
