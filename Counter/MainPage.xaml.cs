using System.Collections.ObjectModel;
using System.IO;

namespace Counter;

public partial class MainPage : ContentPage
{
    private readonly string _filePath = Path.Combine(FileSystem.AppDataDirectory, "counters.txt");
    private ObservableCollection<CounterModel> _counters;

    public MainPage()
    {
        InitializeComponent();
        _counters = new ObservableCollection<CounterModel>();

        LoadCountersFromFile();
    }

    private void SaveCountersToFile()
    {
        var lines = new List<string>();

        foreach (var counter in _counters)
        {
            lines.Add(counter.Name);          
            lines.Add(counter.Value.ToString());
        }

        File.WriteAllLines(_filePath, lines);
    }

    private void LoadCountersFromFile()
    {
        if (File.Exists(_filePath))
        {
            var lines = File.ReadAllLines(_filePath);

            for (int i = 0; i < lines.Length; i += 2)
            {
                if (i + 1 < lines.Length && int.TryParse(lines[i + 1], out int value))
                {
                    var counter = new CounterModel(lines[i]) { Value = value };
                    _counters.Add(counter);
                    AddCounterToUI(counter);
                }
            }
        }
    }

    private async void OnAddCounterClicked(object sender, EventArgs e)
{
    string counterName = await DisplayPromptAsync("New Counter", "Provide a name for the counter:", initialValue: "Counter");

    if (string.IsNullOrWhiteSpace(counterName))
    {
        await DisplayAlert("Error", "The counter name can't be empty!", "OK");
        return;
    }

    string valueInput = await DisplayPromptAsync("New Counter", "Provide an initial value for the counter:", initialValue: "0");

    if (!int.TryParse(valueInput, out int initialValue))
    {
        await DisplayAlert("Error", "Initial value must be a valid number!", "OK");
        return;
    }

    var newCounter = new CounterModel(counterName) { Value = initialValue };

    _counters.Add(newCounter);
    AddCounterToUI(newCounter);

    SaveCountersToFile();
}


    private void AddCounterToUI(CounterModel counter)
    {
        var counterLayout = new HorizontalStackLayout
        {
            Spacing = 10,
            VerticalOptions = LayoutOptions.Center
        };

        var counterLabel = new Label
        {
            Text = $"{counter.Name}: {counter.Value}",
            FontSize = 24,
            VerticalOptions = LayoutOptions.Center
        };

        var increaseButton = new Button
        {
            Text = "+",
            FontSize = 24,
            WidthRequest = 50
        };
        increaseButton.Clicked += (s, e) => {
            counter.Value++;
            counterLabel.Text = $"{counter.Name}: {counter.Value}";
            SaveCountersToFile(); 
        };

        var decreaseButton = new Button
        {
            Text = "-",
            FontSize = 24,
            WidthRequest = 50
        };
        decreaseButton.Clicked += (s, e) => {
            counter.Value--;
            counterLabel.Text = $"{counter.Name}: {counter.Value}";
            SaveCountersToFile(); 
        };

        counterLayout.Children.Add(decreaseButton);
        counterLayout.Children.Add(counterLabel);
        counterLayout.Children.Add(increaseButton);

        CountersStack.Children.Add(counterLayout);
    }
}
