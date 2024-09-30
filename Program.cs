//У вас есть список URL-адресов, которые нужно загрузить параллельно (загрузить содержимое страницы), 
//используя несколько потоков на C#. В программе должна быть возможность отменить операцию,
//если она занимает слишком много времени или если пользователь решит ее отменить. Напишите программу,
//которая загружает файлы, используя TPL и CancellationToken.

//class Program
//{
//    static async Task Main(string[] args)
//    {
//        List<string> urls = new List<string>
//        {
//            "https://www.google.com.ua",
//            "https://gefest.ua",
//            "https://unity.com"
//        };


//        using (CancellationTokenSource cts = new CancellationTokenSource())
//        {
//            Task.Run(() =>
//            {
//                Console.WriteLine("Нажмите 'c', чтобы отменить загрузку или подождите 5 секунд.");
//                if (Console.ReadKey().KeyChar == 'c')
//                {
//                    cts.Cancel();
//                    Console.WriteLine("\nОтмена операции...");
//                }
//            });

//            try
//            {
//                await DownloadUrlsAsync(urls, cts.Token);
//            }
//            catch (OperationCanceledException)
//            {
//                Console.WriteLine("Операция была отменена.");
//            }
//        }
//    }
//    static async Task DownloadUrlsAsync(List<string> urls, CancellationToken cancellationToken)
//    {
//        using HttpClient httpClient = new HttpClient();
//        List<Task> downloadTasks = new List<Task>();

//        foreach (string url in urls)
//        {
//            downloadTasks.Add(DownloadUrlAsync(httpClient, url, cancellationToken));
//        }

//        await Task.WhenAll(downloadTasks);
//    }

//    static async Task DownloadUrlAsync(HttpClient httpClient, string url, CancellationToken cancellationToken)
//    {
//        cancellationToken.ThrowIfCancellationRequested();

//        try
//        {
//            Console.WriteLine($"Начинаем загрузку: {url}");
//            HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken);
//            string content = await response.Content.ReadAsStringAsync(cancellationToken);
//            Console.WriteLine($"Загружено {url}: {content.Length} символов.");
//        }
//        catch (TaskCanceledException)
//        {
//            Console.WriteLine($"Загрузка отменена: {url}");
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Ошибка при загрузке {url}: {ex.Message}");
//        }
//    }
//}

    // ДОП 1 
    //У вас есть массив целых чисел, и вам необходимо вычислить сумму всех элементов массива. 
    //Напишите параллельную программу для вычисления суммы с использованием нескольких объектов класса Task.
//class Program
//{
//    static void Main(string[] args)
//    {

//        int[] numbers = new int[2000];
//        for(int i = 0; i < numbers.Length; i++)
//        {
//            numbers[i]=new Random().Next(1,500);
//        }

//        int taskCount = 4;

//       int totalSum = ParallelSum(numbers, taskCount);

//        Console.WriteLine($"Сумма всех элементов массива: {totalSum}");
//    }

//    static int ParallelSum(int[] array, int taskCount)
//    {
//        int length = array.Length;
//        int chunkSize = length / taskCount;

//        Task<int>[] tasks = new Task<int>[taskCount];

//        for (int i = 0; i < taskCount; i++)
//        {
//            int start = i * chunkSize;
//            int end = (i == taskCount - 1) ? length : start + chunkSize;
//            tasks[i] = Task<int>.Factory.StartNew(() => SumRange(array, start, end));
//        }
//        Task.WaitAll(tasks);
//        int totalSum = tasks.Sum(t => t.Result);
//        return totalSum;
//    }
//    static int SumRange(int[] array, int start, int end)
//    {
//        int sum = 0;
//        for (int i = start; i < end; i++)
//        {
//            sum += array[i];
//        }
//        return sum;
//    }
//}

//ДОП 2 
//Создайте приложение для работы с массивом: 

//■ Удаление из массива повторяющихся значений; 
//■ Сортировка массива(стартует после удаления дублей); 
//■ Бинарный поиск некоторого значения (стартует после сортировки). 

//Используйте «Continuation Tasks» для решения поставленной задачи.

class Program
{
    static void Main(string[] args)
    {
        int[] numbers = new int[2000];
        for (int i = 0; i < numbers.Length; i++)
        {
            numbers[i] = new Random().Next(1, 500);
        }

        //■ Удаление из массива повторяющихся значений; 
        Task<int[]> removeDuplicatesTask = Task.Run(() =>
        {
            int[] distinctArray = numbers.Distinct().ToArray();
            return distinctArray;
        });

        //■ Сортировка массива(стартует после удаления дублей); 
        Task<int[]> sortTask = removeDuplicatesTask.ContinueWith(t =>
        {
            int[] sortedArray = t.Result.OrderBy(x => x).ToArray();
            return sortedArray;
        });

        //■ Бинарный поиск некоторого значения (стартует после сортировки).
        Task binarySearchTask = sortTask.ContinueWith(t =>
        {
            int[] sortedArray = t.Result;
            int valueToFind = 300;
            int index = Array.BinarySearch(sortedArray, valueToFind);

            if (index >= 0)
            {
                Console.WriteLine($"Значение {valueToFind} найдено на индексе {index}");
            }
            else
            {
                Console.WriteLine($"Значение {valueToFind} не найдено");
            }
        });
        binarySearchTask.Wait();

        Console.WriteLine("END.");
    }
}