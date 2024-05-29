using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using AppKit;
using CoreGraphics;
using Foundation;

namespace Sorter
{
    public partial class ViewController : NSViewController
    {
        private const int maxGraphElements = 200;
        private bool arrayRangeFilled = false;
        private bool arraySizeFilled = false;
        private bool sortingInProgress = false;
        private bool arraySorted = false;

        private string generatedArrayString = string.Empty;
        private string sortedArrayString = string.Empty;
        private int sortingComplexity = 0;
        private TimeSpan sortingTime;

        public ViewController(IntPtr handle) : base(handle) { }

        public override void ViewDidLoad()
        {
            base.ViewDidLoad();
            SetupComboBox();

            arraySize.EditingEnded += (sender, e) => HandleEditingEnded(arraySize);
            arrayRange.EditingEnded += (sender, e) => HandleEditingEnded(arrayRange);

            NSNotificationCenter.DefaultCenter.AddObserver(new NSString("AppleInterfaceThemeChangedNotification"), AppearanceChangedNotification);
        }

        private void AppearanceChangedNotification(NSNotification notification)
        {
            RedrawArray();
        }

        private void SetupComboBox()
        {
            string[] items = { "100", "200", "500", "1000", "2000", "5000", "10000", "20000", "50000" };
            arraySize.RemoveAll();
            int maxItemsToShow = 100;
            for (int i = 0; i < Math.Min(maxItemsToShow, items.Length); i++)
            {
                arraySize.Add(new NSString(items[i]));
            }
        }

        private void HandleEditingEnded(NSTextField textField)
        {
            if (textField == arrayRange)
            {
                arrayRangeFilled = !string.IsNullOrWhiteSpace(arrayRange.StringValue);
            }
            else if (textField == arraySize)
            {
                arraySizeFilled = !string.IsNullOrWhiteSpace(arraySize.StringValue);
            }

            if (arrayRangeFilled && arraySizeFilled)
            {
                ValidateAndShowErrors();
            }
        }

        private bool ValidateInput(out int range, out int size)
        {
            range = 0;
            size = 0;

            if (!int.TryParse(arrayRange.StringValue, out range) || !int.TryParse(arraySize.StringValue, out size))
            {
                ShowAlert("Помилка", "Некоректне введення. Введіть правильні числові значення розмірності масиву та діапазон генерації.");
                return false;
            }

            if (size < 100 || size > 50000)
            {
                ShowAlert("Помилка", "Розмір масиву повинен бути в межах від 100 до 50000.");
                return false;
            }

            if (size <=0 || range <= 0)
            {
                ShowAlert("Помилка", "Ви не можете ввести число менше або рівне нулю.");
                return false;
            }

            if (range > size)
            {
                ShowAlert("Помилка", "Діапазон повинен бути меншим за обрану кількість елементів.");
                return false;
            }

            return true;
        }


        private void ValidateAndShowErrors()
        {
            ValidateInput(out _, out _);
        }

        partial void GenerateArray(NSObject sender)
        {
            if (!ValidateInput(out int range, out int size))
            {
                return;
            }

            int[] array = new int[size];
            string selectedOrder = sortOrder.TitleOfSelectedItem;

            if (selectedOrder == "Прямий")
            {
                for (int i = 0; i < size; i++)
                {
                    array[i] = i;
                }
            }
            else if (selectedOrder == "Зворотний")
            {
                for (int i = 0; i < size; i++)
                {
                    array[i] = size - i - 1;
                }
            }
            else
            {
                Random rand = new Random();
                for (int i = 0; i < size; i++)
                {
                    array[i] = rand.Next(0, range + 1);
                }
            }

            generatedArrayString = string.Join(", ", array);
            arrayOutput.StringValue = generatedArrayString;
            DrawArray(array);
        }

        partial void SaveArray(NSObject sender)
        {
            if (string.IsNullOrWhiteSpace(generatedArrayString))
            {
                ShowAlert("Помилка", "Немає згенерованого масиву для збереження.");
                return;
            }

            string[] arrayElements = generatedArrayString.Split(new string[] { ", " }, StringSplitOptions.None);

            if (arrayElements.Length == 0)
            {
                ShowAlert("Помилка", "Немає згенерованого масиву для збереження.");
                return;
            }

            var savePanel = new NSSavePanel
            {
                Title = "Зберегти файл",
                AllowedFileTypes = new string[] { "txt" }
            };

            savePanel.Begin(result =>
            {
                if (result == 1)
                {
                    var url = savePanel.Url;

                    if (url != null)
                    {
                        string filePath = url.Path;

                        string content = $"Згенерований масив:\n{generatedArrayString}\n\n";

                        if (arraySorted)
                        {
                            content += $"Відсортований масив ({sortMethod.TitleOfSelectedItem}):\n{sortedArrayString}\n\n";
                            content += $"Складність: {sortingComplexity}";
                        }
                        else
                        {
                            content += $"Масив не відсортований.";
                        }

                        try
                        {
                            System.IO.File.WriteAllText(filePath, content);
                        }
                        catch (Exception ex)
                        {
                            ShowAlert("Помилка", $"Не вдалося зберегти масиви у файл. Помилка: {ex.Message}");
                        }
                    }
                }
            });
        }

        partial void SortArray(NSObject sender)
        {
            if (!ValidateInput(out int range, out int size))
            {
                return;
            }

            int[] array = Array.ConvertAll(generatedArrayString.Split(new string[] { ", " }, StringSplitOptions.None), int.Parse);

            string selectedMethod = sortMethod.TitleOfSelectedItem;
            string selectedOrder = sortOrder.TitleOfSelectedItem;

            Action<int[]> drawArray = DrawArray;

            sortingInProgress = true;

            Task.Run(() =>
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                int[] sortedArray = null;

                bool worstCase = selectedOrder == "Прямий" || selectedOrder == "Зворотний";

                switch (selectedMethod)
                {
                    case "MergeSort":
                        sortedArray = Mergesort.Sort(array, 0, array.Length - 1, drawArray, 0);
                        sortingComplexity = array.Length * Mergesort.maxRecursionDepth;
                        break;
                    case "QuickSort":
                        if (worstCase)
                        {
                            sortedArray = Quicksort.Sort(array, 0, array.Length - 1, drawArray, 0);
                            sortingComplexity = (array.Length * array.Length);
                        }
                        else
                        {
                            sortedArray = Quicksort.Sort(array, 0, array.Length - 1, drawArray, 0);
                            sortingComplexity = (array.Length * Quicksort.maxRecursionDepth);
                        }
                        break;
                    case "IntroSort":
                        sortedArray = Introsort.IntroSort(array, 0, array.Length - 1, 2 * (int)Math.Log(array.Length), drawArray, 0);
                        sortingComplexity = (array.Length * Introsort.maxRecursionDepth);
                        break;
                    default:
                        sortedArray = Mergesort.Sort(array, 0, array.Length - 1, drawArray, 0);
                        sortingComplexity = array.Length * Mergesort.maxRecursionDepth;
                        break;
                }

                stopwatch.Stop();
                sortingTime = stopwatch.Elapsed;

                InvokeOnMainThread(() =>
                {
                    sortedArrayString = string.Join(", ", sortedArray);
                    arrayOutput.StringValue = sortedArrayString;
                    sortingInProgress = false;
                    arraySorted = true;
                    double seconds = sortingTime.TotalMilliseconds / 1000.0;
                    timeOutput.StringValue = $"{seconds:F3}с";
                });
            });
        }

        private void RedrawArray()
        {
            if (!string.IsNullOrWhiteSpace(generatedArrayString))
            {
                int[] array = Array.ConvertAll(generatedArrayString.Split(new string[] { ", " }, StringSplitOptions.None), int.Parse);
                DrawArray(array);
            }
        }

        private void DrawArray(int[] array)
        {
            InvokeOnMainThread(() =>
            {
            if (sortGraph == null)
            {
                return;
            }

            if (array.Length > maxGraphElements)
            {
                return;
            }

            var size = sortGraph.Frame.Size;
            var image = new NSImage(size);

            if (image == null)
            {
                return;
            }

            image.LockFocus();

            var context = NSGraphicsContext.CurrentContext?.CGContext;
            if (context == null)
            {
                image.UnlockFocus();
                return;
            }

            context.ClearRect(new CGRect(0, 0, size.Width, size.Height));

            double rectWidth = size.Width / array.Length;
            int maxArrayValue = array.Max();
            float scaleY = (float)(size.Height / maxArrayValue);

            NSColor barColor;
            var appearance = NSApplication.SharedApplication.EffectiveAppearance;

            if (appearance.Name == NSAppearance.NameDarkAqua)
            {
                barColor = NSColor.White;
            }
            else
            {
                barColor = NSColor.Blue;
            }

            for (int i = 0; i < array.Length; i++)
            {
                context.SetFillColor(barColor.CGColor);
                context.FillRect(new CGRect(i * rectWidth, 0, rectWidth - 1, array[i] * scaleY));
            }

            image.UnlockFocus();
            sortGraph.Image = image;
            });
        }

        private void ShowAlert(string title, string message)
        {
            var alert = new NSAlert
            {
                AlertStyle = NSAlertStyle.Critical,
                InformativeText = message,
                MessageText = title
            };
            alert.RunModal();
        }
    }
}
