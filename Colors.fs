module ColorSquaresApp.Colors

open Xamarin.Forms

// Предопределенная палитра красивых цветов
let predefinedColors = [
    Color.FromHex("#FF6B6B")  // Красный
    Color.FromHex("#4ECDC4")  // Бирюзовый
    Color.FromHex("#45B7D1")  // Голубой
    Color.FromHex("#96CEB4")  // Зеленый
    Color.FromHex("#FFEAA7")  // Желтый
    Color.FromHex("#DDA0DD")  // Сливовый
    Color.FromHex("#98D8C8")  // Мятный
    Color.FromHex("#F7DC6F")  // Золотой
    Color.FromHex("#BB8FCE")  // Фиолетовый
    Color.FromHex("#85C1E9")  // Небесный
    Color.FromHex("#F8C471")  // Оранжевый
    Color.FromHex("#82E0AA")  // Светло-зеленый
]

let getRandomPredefinedColor () =
    let random = System.Random()
    predefinedColors.[random.Next(predefinedColors.Length)]