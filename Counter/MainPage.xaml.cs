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
        var lines = _counters.Select(c => $"{c.Name}|{c.Value}").ToList();
        File.WriteAllLines(_filePath, lines);
    }

    private void LoadCountersFromFile()
    {
        if (File.Exists(_filePath))
        {
            var lines = File.ReadAllLines(_filePath);
            foreach (var line in lines)
            {
                var parts = line.Split('|');
                if (parts.Length == 2 && int.TryParse(parts[1], out int value))
                {
                    var counter = new CounterModel(parts[0]) { Value = value };
                    _counters.Add(counter);
                    AddCounterToUI(counter); 
                }
            }
        }
    }

    private async void OnAddCounterClicked(object sender, EventArgs e)
    {
        string input = await DisplayPromptAsync("New Counter", "Provide a name and initial value for the counter in the format: Name|Value:", initialValue: "Counter|0");

        if (string.IsNullOrWhiteSpace(input))
        {
            await DisplayAlert("Error", "Name and initial value of the counter can't be empty!", "OK");
            return;
        }

        var parts = input.Split('|');

        if (parts.Length != 2 || string.IsNullOrWhiteSpace(parts[0]))
        {
            await DisplayAlert("Error", "You must provide both the counter name and the initial value in the format: name|value", "OK");
            return;
        }

        string counterName = parts[0];

        if (!int.TryParse(parts[1], out int initialValue))
        {
            await DisplayAlert("Error", "Initial value must be a number!", "OK");
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
