using Menu.Service;
using System.Collections.ObjectModel;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;

namespace Menu.ViewModel;

public class MainPageViewModel
{
    public class CategoryMenu(string category, string choices)
    {
        public string Category { get; set; } = category;
        public string Choices { get; set; } = choices;
    }

    private readonly FoodCategoryService _foodCategoryService;
    private readonly FoodItemService _foodItemService;
    private readonly INavigationService _navigationService;
    private readonly IDatabaseService _databaseService;

    public ICommand RefreshCommand { get; }

    public ICommand OpenCategoryPageCommand => new Command(async () =>
    {
        await _navigationService.NavigateToFoodCategoryPage();
    });

    private Dictionary<string, Tuple<Dictionary<string, int>, List<List<string>>, int>>? data;

    public ObservableCollection<CategoryMenu> Menu { get; private set; } = [];

    private static readonly Random random = new();

    public MainPageViewModel(FoodCategoryService foodCategoryService, FoodItemService foodItemService, INavigationService navigationService, IDatabaseService databaseService)
    {
        _foodCategoryService = foodCategoryService;
        _foodItemService = foodItemService;
        _navigationService = navigationService;
        _databaseService = databaseService;
        RefreshCommand = new Command(RefreshMenu);
    }

    public async Task InitializeAsync()
    {
        data = await GetDataAsync();
        RefreshMenu();
    }

    public async Task<Dictionary<string, Tuple<Dictionary<string, int>, List<List<string>>, int>>> GetDataAsync()
    {
        var categories = new Dictionary<string, Tuple<Dictionary<string, int>, List<List<string>>, int>>();
        var categoryList = await _foodCategoryService.GetCategoriesAsync();
        foreach (var category in categoryList)
        {
            var itemsList = await _foodItemService.GetItemsByCategoryIdAsync(category.Id);
            var foodDict = itemsList.ToDictionary(item => item.FoodName, item => (int)item.Weight);
            var groups = itemsList
                .Where(item => item.GroupNumber != 0)
                .GroupBy(item => item.GroupNumber)
                .Select(grp => grp.Select(item => item.FoodName).ToList())
                .ToList();
            categories.Add(category.CategoryName, new Tuple<Dictionary<string, int>, List<List<string>>, int>(foodDict, groups, category.NumberOfItems));
        }
        return categories;
    }

    public void RefreshMenu()
    {
        if (data != null)
        {
            Menu.Clear();
            foreach (var category in data.Keys)
            {
                var (options, groups, times) = data[category];
                var newChoices = RandomChoices(options, groups, times);
                Menu.Add(new CategoryMenu(category, string.Join("、", newChoices)));
            }
        }
    }

    public static List<string> RandomChoices(Dictionary<string, int> options, List<List<string>> groups, int times)
    {
        var keys = options.Keys.ToList();
        var weights = options.Values.ToList();
        var choices = new List<string>();

        while (choices.Count < times && keys.Count > 0)
        {
            int index = GetWeightedRandomIndex(weights);
            string choice = keys[index];
            choices.Add(choice);

            var group = GetGroup(choice, groups);
            foreach (string item in group)
            {
                int itemIndex = keys.IndexOf(item);
                if (itemIndex != -1)
                {
                    keys.RemoveAt(itemIndex);
                    weights.RemoveAt(itemIndex);
                }
            }
        }

        return choices;
    }

    private static List<string> GetGroup(string choice, List<List<string>> groups)
    {
        foreach (List<string> group in groups)
        {
            if (group.Contains(choice))
            {
                return group;
            }
        }
        return [choice];
    }

    private static int GetWeightedRandomIndex(List<int> weights)
    {
        int totalWeight = weights.Sum();
        int randomWeight = random.Next(totalWeight);
        int cumulativeWeight = 0;
        for (int i = 0; i < weights.Count; i++)
        {
            cumulativeWeight += weights[i];
            if (randomWeight < cumulativeWeight)
            {
                return i;
            }
        }
        return weights.Count - 1;
    }

    public async Task<bool> ImportData()
    {
        try
        {
            string? clipboardContent = await Clipboard.GetTextAsync() ?? throw new Exception();
            var databaseData = Convert.FromBase64String(DecompressText(clipboardContent, "menu"));

            await _databaseService.CloseConnectionAsync();
            if (File.Exists(_databaseService.DatabasePath))
            {
                File.Delete(_databaseService.DatabasePath);
            }
            File.WriteAllBytes(_databaseService.DatabasePath, databaseData);
            _databaseService.OpenConnection();

            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> ExportData()
    {
        try
        {
            await _databaseService.CloseConnectionAsync();
            string base64File = Convert.ToBase64String(File.ReadAllBytes(_databaseService.DatabasePath));
            await Clipboard.SetTextAsync(CompressText(base64File, "menu"));
            _databaseService.OpenConnection();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public static string CompressText(string text, string password)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(text);
        MemoryStream compressionStream = new();
        using (GZipStream zip = new(compressionStream, CompressionMode.Compress, true))
        {
            zip.Write(buffer, 0, buffer.Length);
        }

        compressionStream.Position = 0;
        byte[] compressed = compressionStream.ToArray();
        byte[] ciphertext = Encrypt(compressed, password);
        return Convert.ToBase64String(ciphertext);
    }

    public static string DecompressText(string compressedText, string password)
    {
        byte[] ciphertext = Convert.FromBase64String(compressedText);
        byte[] compressed = Decrypt(ciphertext, password);
        using MemoryStream decompressionStream = new(compressed);
        using GZipStream zip = new(decompressionStream, CompressionMode.Decompress);
        byte[] buffer = new byte[1024];
        int bytesRead;
        using MemoryStream decompressed = new();
        while ((bytesRead = zip.Read(buffer, 0, buffer.Length)) > 0)
        {
            decompressed.Write(buffer, 0, bytesRead);
        }
        return Encoding.UTF8.GetString(decompressed.ToArray());
    }

    public static byte[] Encrypt(byte[] data, string password)
    {
        using var aes = Aes.Create();
        var salt = new byte[16];
        using (var rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(salt);
        }

        using (var passwordDeriveBytes = new Rfc2898DeriveBytes(password, salt, iterations: 10000, HashAlgorithmName.SHA256))
        {
            aes.Key = passwordDeriveBytes.GetBytes(32);
            aes.IV = passwordDeriveBytes.GetBytes(16);
        }

        using var encryptionStream = new MemoryStream();
        encryptionStream.Write(salt, 0, salt.Length);

        using (var cryptoStream = new CryptoStream(encryptionStream, aes.CreateEncryptor(), CryptoStreamMode.Write))
        {
            cryptoStream.Write(data, 0, data.Length);
            cryptoStream.Close();
        }

        return encryptionStream.ToArray();
    }

    public static byte[] Decrypt(byte[] data, string password)
    {
        using var aes = Aes.Create();
        var salt = new byte[16];
        Buffer.BlockCopy(data, 0, salt, 0, salt.Length);

        using (var passwordDeriveBytes = new Rfc2898DeriveBytes(password, salt, iterations: 10000, HashAlgorithmName.SHA256))
        {
            aes.Key = passwordDeriveBytes.GetBytes(32);
            aes.IV = passwordDeriveBytes.GetBytes(16);
        }

        using var decryptionStream = new MemoryStream();
        using (var cryptoStream = new CryptoStream(decryptionStream, aes.CreateDecryptor(), CryptoStreamMode.Write))
        {
            cryptoStream.Write(data, salt.Length, data.Length - salt.Length);
            cryptoStream.Close();
        }

        return decryptionStream.ToArray();
    }

}
