
//
// dot net plugins are supported in LCD Smartie 5.3 beta 3 and above.
//
// ====  Плагин "txt_manager" - плагин работы с текстом. ====
// Автор - Сапунов Д.В. 
// Кемерово 2022 г.
// You may provide/use upto 20 functions (function1 to function20).


using System;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace txt_manager
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class LCDSmartie
	{
        public LCDSmartie()
		{
            for (int i = 0; i < old_speed.Length; i++){ old_speed[i] = 100; }// - каждый старый счётчик при старте равен 100, для мгновенного входа в условие - if (old_speed[number] >= speed)
        }
        static int element = 50; // - число независимых счётчиков (общий параметр для всех функций)
        ASII tabl = new ASII();                                 // - таблица подмены символов
        ASII_transliteration ASII = new ASII_transliteration(); // - класс для транслитерациии
        string old_txt = ""; // - прежняя строка
        int old_number = 0;  // - прежний номер
        private int number_txt(string param1)
        {
            if (old_txt == param1) { return old_number; }// - если пришла прежняя строка возвращаем старый номер счётчика
            int frch = 0;
            foreach (string id_txt in in_txt)
            {
                if (id_txt == param1) {  break; }   // - если нашли выходим
                if (id_txt == null) { in_txt[frch] = param1;  counter[frch] = 0; break; }// - если ссылку не нашли, то добавлем её на первое свободное место в массиве
                frch++;
                if (frch == in_txt.Length) { for (int i = 0; i < in_txt.Length; i++) { in_txt[i] = null; } }  // - если достигнут предел числа ссылок очищаем массив ссылок
            }
            old_txt = param1;
            old_number = frch;
            return frch;
        }

        // ======================= Функция для добавления пустых символов в строку ==================================
        //
        private string param1_processing(string param1, string param2)
        {
            string[] sub_prm2 = param2.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);// - массив с парамметрами
            string beginning = "";// - строка в начале
            string end = "";      // - строка в конце
            width = Convert.ToInt32(sub_prm2[0]);
            //------------------------- Добавление пустых символов в начале и в конце -----------------------------------
            for (int i = 0; i < width; i++)
            {
                if (i % 2 == 0) { beginning += " "; } // - формируем пустые символы в начале, так что-бы строка начинала двигаться с середины
                end += " ";                          // - формируем пустые символы в конце. Число символв равно ширине блока.
            }
            if (sub_prm2[2] == "R" || sub_prm2[2] == "TrR") { end = beginning = " "; } // - добавим пустые символы в начале и в конце если включен реверс
            if (param1.Length <= width) { width = param1.Length; end = beginning = ""; } // "заморозим" строку если строка короче блока
            param1 = beginning + param1 + end;             // - добавляем пустые символы в начале и в конце
            //-----------------------------------------------------------------------------------------------------------
            return param1;
        }

        int[] old_speed = new int[element];    // - флаг скорости для 1-ой функции
        string[] exit   = new string[element]; // - выходной текст
        string[] in_txt = new string[element]; // - текст на входе, для сохранения уникального номера счётчика
        int[] counter   = new int[element];    // - массив с счётчиками 
        int lenght_text = 0;                   // - длинна строки (колличество символов)
        int revers_flag = 0;                   // - флаг реверса 
        int width  = 24;                       // - ширина текстового блока

        // ============================ Функция №1 выводит текст бегущей строкой. ======================================
        // $dll(txt_manager, 1, "текст", #  ширина_блока # скорость # реверс #.)
        public string function1(string param1, string param2)
        {
            /*
             *       sub_prm2[0] = ширина текстового блока
             *       sub_prm2[1] = скорость сдвига
             *       sub_prm2[2] = "R"   - запустить реверс текстовой строки (строка замораживается на месте если она короче блока)                     
             *                     "Tr"  - транслитерация русского текста
             *                     "TrR" - транслитерация русского текста с реверсом
             *            param2 = "tc" - длинна строки
             *                     "сс" - текущее положение строки
             *                      "%" - вернуть процент прочитанного текста
             */
            string[] sub_prm2 = param2.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);// - массив с парамметрами
            int number;  // - номер счётчика  
            int speed;  // - скорость бегущей строки
            int char_poz;  // - порядковый номер символа в строке
            int temp;  // - для расчётов
            try
            {
                number = number_txt(param1);// - запрос номера счётчика                            
                if (param2 == "help") { return function4("help", "");}
                if (param2 == "about") { return function4("about", ""); }
                if (param2 == "%")  { temp = counter[number] * 100 / (lenght_text - width); return temp.ToString(); } // - возвращаем процент прочитанного текста
                if (param2 == "tc") { return lenght_text.ToString(); }     // - возвращаем длинну строки
                if (param2 == "cc") { return counter[number].ToString(); } // - возвращаем текущее положение строки

                if (sub_prm2[2] == "Tr" || sub_prm2[2] == "TrR") { param1 = ASII.transliton(param1); }  // - транслитерация русских слов

                width  = Convert.ToInt32(sub_prm2[0]);                     // - установка ширины блока                                    
                speed  = Convert.ToInt32(sub_prm2[1]);                     // - установка скорости прокрутки
                param1 = param1_processing(param1, param2);                // - добавление пустых символов в начале и в конце
               
                if (param2 != "%") // - возвращаем текст
                {
                    byte[] asciiBytes = Encoding.Default.GetBytes(param1); // - разбиваем текст на массив с кодами символов
                    lenght_text = asciiBytes.Length;                       // - длинна строки 
                    if (counter[number] > (lenght_text - width)) { counter[number] = 0; revers_flag++; if (revers_flag == 2) { revers_flag = 0; } }// - сбрасываем счётчик длинны строки и флаг реверса если дошли до конца строки
                    if (old_speed[number] >= speed)                        // - регулируем скорость вывода
                    {
                        exit[number] = "";                                 // - очистка выходной строки перед заполнением 
                        for (int i = 0; i < width; i++)                    // - вывод символов на ширину блока
                        {
                            char_poz = i + counter[number];                // - положение символа в массиве для движения в лево
                            if ((sub_prm2[2] == "R" || sub_prm2[2] == "TrR") && revers_flag == 1){ char_poz = i + (lenght_text - counter[number] - width); }// - положение символа в массиве для движения в право (реверс)
                            asciiBytes[char_poz] = tabl.ASII_table[asciiBytes[char_poz]];    // - прогонка символов через таблицу подмены                             
                            exit[number] += "$Chr(" + asciiBytes[char_poz].ToString() + ")"; // - заполняем выходную строку символами 
                        }
                        old_speed[number] = 0; // - сброс флага скорости
                        counter[number]++;     // - считаем символы
                        return exit[number];   // - возвращаем текст набором символов
                    }
                }
                old_speed[number]++;
                return exit[number]; // - возвращаем текст набором символов
              
            }
            catch {return function1("Ошибка плагина - txt_manager.dll! Неверная запись - " + param2 + ".", "24#1#0");}
        }
        //================================================================================================================
        //
        
        int[] old_speed_2 = new int[element];   // - флаг скорости для 2-ой функции   
        int number_2 = 0;                       // - номер счётчика для 2-й функции 
        int width_2  = 24;                      // - ширина текстового блока для 2-й функции
        int speed_2 = 1;                        // - скорость перелистывания для 2-й функции
        int total_lines = 0;                    // - всего линий, которые будут выводится
        string[] stroka = new string[7];        // - строки
        int procent = 0;
        int curentPage = 0;
        int totalPage = 0;
        // ============================== Функция №2 выводит текст блоком. =============================================
        // $dll(txt_manager, 2, "текст", строк_всего # номер_текущей_строки  # ширина_блока # скорость # транслитерация(если нужно) # тип_прокрутки
        // пример использования $dll(txt_manager, 2, Привет Мир! Я строка из недр этого компьютера., 1#1#24#20#0)
        // $dll(txt_manager, 2, Привет Мир! Я строка из недр этого компьютера., 1#1#24#20#Tr) - с транслитерацией
        public string function2(string param1, string param2)
        {
            /* Описание sub_prm2[] */ /*       
             *       sub_prm2[0] = число строк
             *       sub_prm2[1] = номер строки
             *       sub_prm2[2] = ширина текстового блока
             *       sub_prm2[3] = скорость перелистывания 
             *       sub_prm2[4] = Tr транслитерация русского текста 
             *       sub_prm2[5] = Ln - построчная прокрутка, Sn - пркрутка змейкой, без параметров - постранично
             *       param2 = %  - вывести процент прочитанного текста
             *       param2 = tp - вывести число страниц
             *       param2 = cp - вывести номер текущей страницы   
             */
            string[] sub_prm2 = param2.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);// - массив с парамметрами
            int nmb_line;    // - номер строки  
            int full_page;   // - максимальное число символв на странице;
            int nul_char = 0;    // - число недостающих символов в конце строки для ровного счёта страниц
            
            if (param2 == "%")     { return procent.ToString(); }    // - возвращаем процент прочитанного текста
            if (param2 == "tp")    { return totalPage.ToString(); }  // - возвращаем число страниц
            if (param2 == "cp")    { return curentPage.ToString(); } // - возвращаем номер текущей страницы
            if (param2 == "cp/tp") { return curentPage.ToString() + "/" + totalPage.ToString();}
            try
            {
                nmb_line = Convert.ToInt32(sub_prm2[1]);// - номер линии
                
                if (sub_prm2[1] == "1")// - если пришла первая строка, "мутим" весь код
                {
                    total_lines = Convert.ToInt32(sub_prm2[0]);
                    number_2 = number_txt(param1); 
                    if (sub_prm2[4] == "Tr") { param1 = ASII.transliton(param1); } // - транслитерация русских слов
                    width_2 = Convert.ToInt32(sub_prm2[2]);
                    speed_2 = Convert.ToInt32(sub_prm2[3]);
                    full_page = width_2 * total_lines;                        // - максимальное число символв на странице
                    if (param1.Length % full_page != 0) {
                        nul_char = full_page - (param1.Length % full_page); } // - число недостающих символов
                    for (int i = 0; i < nul_char; i++) { param1 += " "; }     // - формируем пустые символы в конце
                    lenght_text = param1.Length;                              // - длина строки 
                    procent = (counter[number_2] / (width_2 * total_lines) * 100) / (lenght_text / (width_2 * total_lines));
                    curentPage = (counter[number_2] / (width_2 * total_lines)) + 1;
                    totalPage = lenght_text / (width_2 * total_lines);
                    try 
                    {
                        if (sub_prm2[5] == "Ln") // - включаем построчную прокруку при условии, что sub_prm2[5] == "Ln" 
                        {
                            for (int i = 0; i < width_2 * total_lines; i++) { param1 = " " + param1 + " "; }
                            full_page = width_2;
                            lenght_text = param1.Length - (width_2 * total_lines) - nul_char;
                        }
                        if (sub_prm2[5] == "Sn") // - включаем прокрутку текста "змейкой", елси sub_prm2[5] == "Sn" 
                        {
                            for (int i = 0; i < width_2 * total_lines; i++) { param1 = " " + param1 + " "; }
                            full_page = 1;
                            lenght_text = param1.Length - (width_2 * (total_lines - 1)) - nul_char;
                        }
                    }
                    catch { }
                    byte[] asciiBytes = Encoding.Default.GetBytes(param1);    // - разбиваем текст на массив с кодами символов
                    
                    if (param1.Length < width_2) { width_2 = param1.Length; } // - если длинна строки меньше ширины блока замораживаем перелистывание
                    if (old_speed_2[number_2] >= speed_2) { old_speed_2[number_2] = 0; counter[number_2] += full_page; } // - регулируем скорость обновления
                    if (counter[number_2] >= (lenght_text - width_2)) { counter[number_2] = 0; }   // - сбрасываем счётчик длины строки если дошли до конца строки

                    for (int l = 1; l <= total_lines; l++)
                    {
                        stroka[l] = "";
                        for (int i = (width_2 * (l - 1)) + counter[number_2]; i < (width_2 * l) + counter[number_2]; i++)// - вывод символов на ширину блока
                        {
                            if (i >= asciiBytes.Length) {break;}                  // - выходим из цикла если i превышает размер массива asciiBytes (такое происходит при смене ширины блока в приложении, что приводит к ошибке в конце текста)
                            asciiBytes[i] = tabl.ASII_table[asciiBytes[i]];       // - прогонка символов через таблицу подмены
                            stroka[l] += "$Chr(" + asciiBytes[i].ToString() + ")";// - заполняем выходную строку символами 
                        }
                    }
                }
            if (nmb_line == total_lines) { old_speed_2[number_2]++; } // - если пришла последняя строка обновляем счётчик скорости и в следущем цикле выводим новую страницу
            return stroka[nmb_line];   // - возвращаем текст по номеру строки 
            }
            catch { return function1("Ошибка плагина - txt_manager.dll! Неверная запись - " + param2 + ".", "24#1#0"); }
        }

      
        // ======================= Функция №3 декодирования символов в HTML формате ====================================
        //
        public string function3(string param1, string param2)
        {
            /*
           * Лютый изврат. Ради одной радиостанции - "Ваня".
           * Название треков передаётся в HTML формате - (к примеру &#1040; -"А").
           * Функция ищет и возвращает всё номерами символов в ASII кодировке. (к примеру "$Chr(192)" - "А";)
           */
            string html_char = "";// - html код символа
            string exit = "";     // - выходная строка
            int int_ = 0;         // - номер символа в ASII кодировке
            int flag = 0;         // - флаг извлечения  html символа
            byte[] asciiBytes = Encoding.Default.GetBytes(param1);// - массив символво в ASII кодировке
            for (int i = 0; i < asciiBytes.Length; i++) // - перебор массива
            {
                /* Значения флага при детектировании html символа
                 * ---------------- &#1040; --------------
                 * flag = 0 - html символ не найден
                 * flag = 1 - найден символ - "&"
                 * flag = 2 - найден символ - "#" сразу после "&", найден символ в html кодировке
                 */

                if (flag == 0) 
                {
                    if(asciiBytes[i] != '&') 
                    exit += "$Chr(" + Convert.ToInt32(asciiBytes[i]) + ")";
                }
                if (asciiBytes[i] != '#' && flag == 1)
                {
                    flag = 0;
                    exit += "$Chr(" + Convert.ToInt32('&') + ")";          // - добавляем символ "&" если он не имеет отношение к html символу
                    exit += "$Chr(" + Convert.ToInt32(asciiBytes[i]) + ")";// - доавляем прочие не html символы
                }
                if (asciiBytes[i] == '&') { flag = 1; }              // - детектор html символа
                if (asciiBytes[i] == '#' && flag == 1) { flag = 2; } //
                if (asciiBytes[i] == ';' && flag == 2) // - обработка полученного html символа            
                { 
                    flag = 0;
                    html_char = html_char.Replace("#", "");
                    html_char = html_char.Replace(";", "");
                    try { int_ = Convert.ToInt32(html_char) - 848; } catch { }
                    exit += "$Chr(" + int_.ToString() + ")";
                    html_char = "";
                }
                if (flag == 2)
                {
                    html_char += Convert.ToChar(asciiBytes[i]); // - формирование html символа
                }
            }
            return exit;
        }   

        
        int open_file_flag = 0; // - флаг разового показа файла "прочти" 

        // ============================== Функция №4 для вызова справки ==============================
        //
        public string function4(string param1, string param2)
        {
            Readmy_txt txt = new Readmy_txt();
            if (param1 == "about") { return function1("Плагин txt_manager V1.0 - работа с текстом. Автор - Сапунов Д.В. email - sdv180@gmail.com. Кемерово 2022 год.", "24#2#0"); }
            if (param1 == "help")
            {
                Encoding win1251 = Encoding.GetEncoding(1251);
                File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/txt_manager.txt", txt.file, win1251);
                if (open_file_flag == 0) { open_file_flag = 1; Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/txt_manager_прочти.txt"); }
                return function1("Читай описание к плагину! Файл -'txt_manager.txt', сохранён на рабочем столе.", "24#2#0");
            }
            return function1("Справка. Узнать больше - укажите param1: 'about','help'.", "24#2#0");
        }


        // ============================== Функция №5 - Обрезать строку ==============================
        //   $dll(txt_manager,5,Привет,3#6) - "ивет"
        //   $dll(txt_manager,5, Привет,1#3) - "При"
        //
        public string function5(string param1, string param2)
        {
            /*   
             *   sub_prm2[0] =  начало обрезки
             *   sub_prm2[1] =  конец
             */
            string[] sub_prm2 = param2.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);// - массив с парамметрами
            string exit = "";
            byte[] asciiBytes = Encoding.Default.GetBytes(param1);
            for (int i = 0; i < asciiBytes.Length; i++)
            {
                if(i >= Convert.ToInt32(sub_prm2[0]) - 1 && i <= Convert.ToInt32(sub_prm2[1]) - 1)
                exit += "$Chr(" + asciiBytes[i].ToString() + ")";
            }
            return exit;
        }

        // ============================== Функция №6 - Удалить часть строки ==============================
        //   $dll(txt_manager,5,Привет,3#6) - "Прит"
        //   $dll(txt_manager,5, Привет,1#3) - "Пивет"
        //
        public string function6(string param1, string param2)
        {
            /*   
             *   sub_prm2[0] =  с какого символа удалить часть строки
             *   sub_prm2[1] =  до какого символа удалить часть строки
             */
            string[] sub_prm2 = param2.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);// - массив с парамметрами
            string exit = "";
            byte[] asciiBytes = Encoding.Default.GetBytes(param1);
            for (int i = 0; i < asciiBytes.Length; i++)
            {
                if (i <= Convert.ToInt32(sub_prm2[0]) - 1 || i >= Convert.ToInt32(sub_prm2[1])-1)
                    exit += "$Chr(" + asciiBytes[i].ToString() + ")";
            }
            return exit;
        }



        public string function7(string param1, string param2)
        {
            
            string[] sub_prm2 = param2.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);// - массив с парамметрами
            
            for (int i = 0; i<sub_prm2.Length; i++)
            {
                if(param1 == sub_prm2[i])
                {
                    return sub_prm2[i + 1];
                }
            }
            return "";
        }

        public int GetMinRefreshInterval()
		{
			return 80; // 300 ms (around 3 times a second)
		}
	}
}
