open System
open System.IO
open System.Windows.Forms
open Newtonsoft.Json

type DictionaryEntry = {
    Word: string
    Definition: string
}


module DigitalDictionary =

    let addEntry word definition (dictionary: Map<string, string>) =
        dictionary |> Map.add word definition

    let updateEntry word newDefinition (dictionary: Map<string, string>) =
        dictionary |> Map.add word newDefinition

    let deleteEntry word (dictionary: Map<string, string>) =
        dictionary |> Map.remove word


    let searchEntry (keyword: string) (dictionary: Map<string, string>) : (string * string) list =
     dictionary
    |> Map.filter (fun (k: string) _ -> k.ToLowerInvariant().Contains(keyword.ToLowerInvariant()))
    |> Map.toList

    let saveToFile filePath (dictionary: Map<string, string>) =
        let json = JsonConvert.SerializeObject(dictionary)
        File.WriteAllText(filePath, json)

    let loadFromFile filePath =
        if File.Exists(filePath) then
            let json = File.ReadAllText(filePath)
            JsonConvert.DeserializeObject<Map<string, string>>(json)
        else
            Map.empty

            
module DictionaryApp =

    open DigitalDictionary

    [<STAThread>]
    let main () =
        let mutable dictionary = Map.empty
        let filePath = "dictionary.json"

        dictionary <- loadFromFile filePath

        let form = new Form(Text = "Digital Dictionary", Width = 600, Height = 400)

        let lblWord = new Label(Text = "Word:", Top = 20, Left = 20)
        let txtWord = new TextBox(Top = 20, Left = 120, Width = 200)

        let lblDefinition = new Label(Text = "Definition:", Top = 60, Left = 20)
        let txtDefinition = new TextBox(Top = 60, Left = 120, Width = 200)

        let btnAdd = new Button(Text = "Add", Top = 100, Left = 20)
        let btnUpdate = new Button(Text = "Update", Top = 100, Left = 100)
        let btnDelete = new Button(Text = "Delete", Top = 100, Left = 200)

        let lblSearch = new Label(Text = "Search:", Top = 140, Left = 20)
        let txtSearch = new TextBox(Top = 140, Left = 120, Width = 200)
        let btnSearch = new Button(Text = "Search", Top = 140, Left = 320)

        let lstResults = new ListBox(Top = 180, Left = 20, Width = 540, Height = 150)

        btnAdd.Click.Add(fun _ ->
            let word = txtWord.Text
            let definition = txtDefinition.Text
            dictionary <- addEntry word definition dictionary
            saveToFile filePath dictionary
            MessageBox.Show("Entry added.") |> ignore)

        btnUpdate.Click.Add(fun _ ->
            let word = txtWord.Text
            let definition = txtDefinition.Text
            dictionary <- updateEntry word definition dictionary
            saveToFile filePath dictionary
            MessageBox.Show("Entry updated.") |> ignore)

        btnDelete.Click.Add(fun _ ->
            let word = txtWord.Text
            dictionary <- deleteEntry word dictionary
            saveToFile filePath dictionary
            MessageBox.Show("Entry deleted.") |> ignore)

        btnSearch.Click.Add(fun _ ->
            let keyword = txtSearch.Text
            let results = searchEntry keyword dictionary
            lstResults.Items.Clear()
            results |> List.iter (fun (word, definition) ->
                lstResults.Items.Add(sprintf "%s: %s" word definition) |> ignore))

        form.Controls.AddRange([|
            lblWord :> Control; txtWord :> Control; 
            lblDefinition :> Control; txtDefinition :> Control; 
            btnAdd :> Control; btnUpdate :> Control; btnDelete :> Control; 
            lblSearch :> Control; txtSearch :> Control; btnSearch :> Control; 
            lstResults :> Control
        |])

        Application.Run(form)

    main()
