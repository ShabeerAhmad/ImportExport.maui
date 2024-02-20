using System.Collections.ObjectModel;
using System.Text;
using System.Xml.Linq;

namespace ImportExport.maui;

public partial class ImportExportPage : ContentPage
{
    public ObservableCollection<string> XMLData { get; set; }
    public ImportExportPage()
    {
        InitializeComponent();

        XMLData = new ObservableCollection<string>();
        xmlListView.ItemsSource = XMLData;
    }

    private async void Import_Clicked(object sender, EventArgs e)
    {
        try
        {
            var options = new PickOptions
            {
                PickerTitle = "Pick an XML file"
            };
            var file = await FilePicker.PickAsync(options);

            if (file != null)
            {
                if (Path.GetExtension(file.FileName).ToLower() == ".xml")
                {
                    await LoadXMLData(file);
                }
                else
                {
                    await DisplayAlert("Invalid File", "Please select an XML file.", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", ex.Message, "OK");
        }
    }

    private async Task LoadXMLData(FileResult file)
    {
        using (Stream stream = await file.OpenReadAsync())
        {
            XMLData.Clear(); // Clear existing data

            XDocument doc = XDocument.Load(stream);

            DisplayXMLData(doc);
        }
    }

    private void DisplayXMLData(XDocument doc)
    {
        string xmlString = doc.ToString();
        XMLData.Add(xmlString);
    }

    private void Export_Clicked(object sender, EventArgs e)
    {
        ExportXmlToFile();
    }
    private void ExportXmlToFile()
    {
        if (XMLData.Count > 0)
        {
            StringBuilder xmlBuilder = new StringBuilder();

            foreach (string item in XMLData)
            {
                xmlBuilder.AppendLine(item);
            }
            string xmlString = xmlBuilder.ToString();

            // Save XML string to file
            string fileName = Guid.NewGuid() + ".xml";
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName);
            File.WriteAllText(filePath, xmlString, Encoding.UTF8);
            DisplayAlert("Export Successful", $"XML data has been exported to: {filePath}", "OK");
        }
        else
        {
            DisplayAlert("Error", "No data availbel for export", "OK");
        }
    }
}