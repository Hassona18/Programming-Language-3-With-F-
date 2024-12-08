open System
open System.IO
open Newtonsoft.Json


type Dictionary = Map<string, string>


let saveToFile (filePath: string) (dictionary: Dictionary) =
    let json = JsonConvert.SerializeObject(dictionary, Formatting.Indented)
    File.WriteAllText(filePath, json)
    printfn "Dictionary saved to %s." filePath


let loadFromFile (filePath: string) =
    if File.Exists(filePath) then
        let json = File.ReadAllText(filePath)
        JsonConvert.DeserializeObject<Dictionary>(json)
    else
        Map.empty


let addWord (dictionary: Dictionary) (word: string) (definition: string) =
    dictionary |> Map.add (word.ToLower()) definition


let updateWord (dictionary: Dictionary) (word: string) (newDefinition: string) =
    if dictionary.ContainsKey(word.ToLower()) then
        dictionary |> Map.add (word.ToLower()) newDefinition
    else
        printfn "Word not found."
        dictionary


let deleteWord (dictionary: Dictionary) (word: string) =
    if dictionary.ContainsKey(word.ToLower()) then
        dictionary |> Map.remove (word.ToLower())
    else
        printfn "Word not found."
        dictionary


let searchWord (dictionary: Dictionary) (keyword: string) =
    dictionary
    |> Map.filter (fun key _ -> key.Contains(keyword.ToLower()))
    |> Map.iter (fun key value -> printfn "%s: %s" key value)

// Main function
[<EntryPoint>]
let main argv =
    let filePath = "dictionary.json"
    let mutable dictionary = loadFromFile filePath

    let rec mainMenu () =
        printfn "\n--- Digital Dictionary ---"
        printfn "1. Add Word"
        printfn "2. Update Word"
        printfn "3. Delete Word"
        printfn "4. Search Word"
        printfn "5. Save & Exit"
        printf "Choose an option: "
        match Console.ReadLine() with
        | "1" ->
            printf "Enter word: "
            let word = Console.ReadLine()
            printf "Enter definition: "
            let definition = Console.ReadLine()
            dictionary <- addWord dictionary word definition
            printfn "Word added."
            mainMenu()
        | "2" ->
            printf "Enter word to update: "
            let word = Console.ReadLine()
            printf "Enter new definition: "
            let newDefinition = Console.ReadLine()
            dictionary <- updateWord dictionary word newDefinition
            mainMenu()
        | "3" ->
            printf "Enter word to delete: "
            let word = Console.ReadLine()
            dictionary <- deleteWord dictionary word
            printfn "Word deleted."
            mainMenu()
        | "4" ->
            printf "Enter word/keyword to search: "
            let keyword = Console.ReadLine()
            printfn "\n--- Search Results ---"
            searchWord dictionary keyword
            mainMenu()
        | "5" ->
            saveToFile filePath dictionary
            printfn "Goodbye!"
        | _ ->
            printfn "Invalid option."
            mainMenu()

    mainMenu()
    0