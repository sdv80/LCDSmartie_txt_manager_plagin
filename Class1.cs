
//
// dot net plugins are supported in LCD Smartie 5.3 beta 3 and above.
//
// ====  Ïëàãèí "txt_manager" - ïëàãèí ðàáîòû ñ òåêñòîì. ====
// Àâòîð - Ñàïóíîâ Ä.Â. 
// Êåìåðîâî 2022 ã.
// You may provide/use upto 20 functions (function1 to function20).


using System;
using System.Text;
using System.IO;
using System.Diagnostics;
using System.IO.Ports;
using System.Text.RegularExpressions;

namespace txt_manager
{
	/// <summary>
	/// Summary description for Class1.
	/// </summary>
	public class LCDSmartie
	{
        public LCDSmartie()
		{
            for (int i = 0; i < old_speed.Length; i++){ old_speed[i] = 100; }// - êàæäûé ñòàðûé ñ÷¸ò÷èê ïðè ñòàðòå ðàâåí 100, äëÿ ìãíîâåííîãî âõîäà â óñëîâèå - if (old_speed[number] >= speed)
        }
        static int element = 50; // - ÷èñëî íåçàâèñèìûõ ñ÷¸ò÷èêîâ (îáùèé ïàðàìåòð äëÿ âñåõ ôóíêöèé)
        ASII tabl = new ASII();                                 // - òàáëèöà ïîäìåíû ñèìâîëîâ
        ASII_transliteration ASII = new ASII_transliteration(); // - êëàññ äëÿ òðàíñëèòåðàöèèè
        string old_txt = ""; // - ïðåæíÿÿ ñòðîêà
        int old_number = 0;  // - ïðåæíèé íîìåð
        private int number_txt(string param1)
        {
            if (old_txt == param1) { return old_number; }// - åñëè ïðèøëà ïðåæíÿÿ ñòðîêà âîçâðàùàåì ñòàðûé íîìåð ñ÷¸ò÷èêà
            int frch = 0;
            foreach (string id_txt in in_txt)
            {
                if (id_txt == param1) {  break; }   // - åñëè íàøëè âûõîäèì
                if (id_txt == null) { in_txt[frch] = param1;  counter[frch] = 0; break; }// - åñëè ññûëêó íå íàøëè, òî äîáàâëåì å¸ íà ïåðâîå ñâîáîäíîå ìåñòî â ìàññèâå
                frch++;
                if (frch == in_txt.Length) { for (int i = 0; i < in_txt.Length; i++) { in_txt[i] = null; } }  // - åñëè äîñòèãíóò ïðåäåë ÷èñëà ññûëîê î÷èùàåì ìàññèâ ññûëîê
            }
            old_txt = param1;
            old_number = frch;
            return frch;
        }

        // ======================= Ôóíêöèÿ äëÿ äîáàâëåíèÿ ïóñòûõ ñèìâîëîâ â ñòðîêó ==================================
        //
        private string param1_processing(string param1, string param2)
        {
            string[] sub_prm2 = param2.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);// - ìàññèâ ñ ïàðàììåòðàìè
            string beginning = "";// - ñòðîêà â íà÷àëå
            string end = "";      // - ñòðîêà â êîíöå
            width = Convert.ToInt32(sub_prm2[0]);
            //------------------------- Äîáàâëåíèå ïóñòûõ ñèìâîëîâ â íà÷àëå è â êîíöå -----------------------------------
            for (int i = 0; i < width; i++)
            {
                if (i % 2 == 0) { beginning += " "; } // - ôîðìèðóåì ïóñòûå ñèìâîëû â íà÷àëå, òàê ÷òî-áû ñòðîêà íà÷èíàëà äâèãàòüñÿ ñ ñåðåäèíû
                end += " ";                          // - ôîðìèðóåì ïóñòûå ñèìâîëû â êîíöå. ×èñëî ñèìâîëâ ðàâíî øèðèíå áëîêà.
            }
            if (sub_prm2[2] == "R" || sub_prm2[2] == "TrR") { end = beginning = " "; } // - äîáàâèì ïóñòûå ñèìâîëû â íà÷àëå è â êîíöå åñëè âêëþ÷åí ðåâåðñ
            if (param1.Length <= width) { width = param1.Length; end = beginning = ""; } // "çàìîðîçèì" ñòðîêó åñëè ñòðîêà êîðî÷å áëîêà
            param1 = beginning + param1 + end;             // - äîáàâëÿåì ïóñòûå ñèìâîëû â íà÷àëå è â êîíöå
            //-----------------------------------------------------------------------------------------------------------
            return param1;
        }

        int[] old_speed = new int[element];    // - ôëàã ñêîðîñòè äëÿ 1-îé ôóíêöèè
        string[] exit   = new string[element]; // - âûõîäíîé òåêñò
        string[] in_txt = new string[element]; // - òåêñò íà âõîäå, äëÿ ñîõðàíåíèÿ óíèêàëüíîãî íîìåðà ñ÷¸ò÷èêà
        int[] counter   = new int[element];    // - ìàññèâ ñ ñ÷¸ò÷èêàìè 
        int lenght_text = 0;                   // - äëèííà ñòðîêè (êîëëè÷åñòâî ñèìâîëîâ)
        int revers_flag = 0;                   // - ôëàã ðåâåðñà 
        int width  = 24;                       // - øèðèíà òåêñòîâîãî áëîêà

        // ============================ Ôóíêöèÿ ¹1 âûâîäèò òåêñò áåãóùåé ñòðîêîé. ======================================
        // $dll(txt_manager, 1, "òåêñò", #  øèðèíà_áëîêà # ñêîðîñòü # ðåâåðñ #.)
        public string function1(string param1, string param2)
        {
            /*
             *       sub_prm2[0] = øèðèíà òåêñòîâîãî áëîêà
             *       sub_prm2[1] = ñêîðîñòü ñäâèãà
             *       sub_prm2[2] = "R"   - çàïóñòèòü ðåâåðñ òåêñòîâîé ñòðîêè (ñòðîêà çàìîðàæèâàåòñÿ íà ìåñòå åñëè îíà êîðî÷å áëîêà)                     
             *                     "Tr"  - òðàíñëèòåðàöèÿ ðóññêîãî òåêñòà
             *                     "TrR" - òðàíñëèòåðàöèÿ ðóññêîãî òåêñòà ñ ðåâåðñîì
             *            param2 = "tc" - äëèííà ñòðîêè
             *                     "ññ" - òåêóùåå ïîëîæåíèå ñòðîêè
             *                      "%" - âåðíóòü ïðîöåíò ïðî÷èòàííîãî òåêñòà
             */
            string[] sub_prm2 = param2.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);// - ìàññèâ ñ ïàðàììåòðàìè
            int number;  // - íîìåð ñ÷¸ò÷èêà  
            int speed;  // - ñêîðîñòü áåãóùåé ñòðîêè
            int char_poz;  // - ïîðÿäêîâûé íîìåð ñèìâîëà â ñòðîêå
            int temp;  // - äëÿ ðàñ÷¸òîâ
            try
            {
                number = number_txt(param1);// - çàïðîñ íîìåðà ñ÷¸ò÷èêà                            
                if (param2 == "help") { return function4("help", "");}
                if (param2 == "about") { return function4("about", ""); }
                if (param2 == "%")  { temp = counter[number] * 100 / (lenght_text - width); return temp.ToString(); } // - âîçâðàùàåì ïðîöåíò ïðî÷èòàííîãî òåêñòà
                if (param2 == "tc") { return lenght_text.ToString(); }     // - âîçâðàùàåì äëèííó ñòðîêè
                if (param2 == "cc") { return counter[number].ToString(); } // - âîçâðàùàåì òåêóùåå ïîëîæåíèå ñòðîêè

                if (sub_prm2[2] == "Tr" || sub_prm2[2] == "TrR") { param1 = ASII.transliton(param1); }  // - òðàíñëèòåðàöèÿ ðóññêèõ ñëîâ

                width  = Convert.ToInt32(sub_prm2[0]);                     // - óñòàíîâêà øèðèíû áëîêà                                    
                speed  = Convert.ToInt32(sub_prm2[1]);                     // - óñòàíîâêà ñêîðîñòè ïðîêðóòêè
                param1 = param1_processing(param1, param2);                // - äîáàâëåíèå ïóñòûõ ñèìâîëîâ â íà÷àëå è â êîíöå
               
                if (param2 != "%") // - âîçâðàùàåì òåêñò
                {
                    byte[] asciiBytes = Encoding.Default.GetBytes(param1); // - ðàçáèâàåì òåêñò íà ìàññèâ ñ êîäàìè ñèìâîëîâ
                    lenght_text = asciiBytes.Length;                       // - äëèííà ñòðîêè 
                    if (counter[number] > (lenght_text - width)) { counter[number] = 0; revers_flag++; if (revers_flag == 2) { revers_flag = 0; } }// - ñáðàñûâàåì ñ÷¸ò÷èê äëèííû ñòðîêè è ôëàã ðåâåðñà åñëè äîøëè äî êîíöà ñòðîêè
                    if (old_speed[number] >= speed)                        // - ðåãóëèðóåì ñêîðîñòü âûâîäà
                    {
                        exit[number] = "";                                 // - î÷èñòêà âûõîäíîé ñòðîêè ïåðåä çàïîëíåíèåì 
                        for (int i = 0; i < width; i++)                    // - âûâîä ñèìâîëîâ íà øèðèíó áëîêà
                        {
                            char_poz = i + counter[number];                // - ïîëîæåíèå ñèìâîëà â ìàññèâå äëÿ äâèæåíèÿ â ëåâî
                            if ((sub_prm2[2] == "R" || sub_prm2[2] == "TrR") && revers_flag == 1){ char_poz = i + (lenght_text - counter[number] - width); }// - ïîëîæåíèå ñèìâîëà â ìàññèâå äëÿ äâèæåíèÿ â ïðàâî (ðåâåðñ)
                            asciiBytes[char_poz] = tabl.ASII_table[asciiBytes[char_poz]];    // - ïðîãîíêà ñèìâîëîâ ÷åðåç òàáëèöó ïîäìåíû                             
                            exit[number] += "$Chr(" + asciiBytes[char_poz].ToString() + ")"; // - çàïîëíÿåì âûõîäíóþ ñòðîêó ñèìâîëàìè 
                        }
                        old_speed[number] = 0; // - ñáðîñ ôëàãà ñêîðîñòè
                        counter[number]++;     // - ñ÷èòàåì ñèìâîëû
                        return exit[number];   // - âîçâðàùàåì òåêñò íàáîðîì ñèìâîëîâ
                    }
                }
                old_speed[number]++;
                return exit[number]; // - âîçâðàùàåì òåêñò íàáîðîì ñèìâîëîâ
              
            }
            catch {return function1("Îøèáêà ïëàãèíà - txt_manager.dll! Íåâåðíàÿ çàïèñü - " + param2 + ".", "24#1#0");}
        }
        //================================================================================================================
        //
        
        int[] old_speed_2 = new int[element];   // - ôëàã ñêîðîñòè äëÿ 2-îé ôóíêöèè   
        int number_2 = 0;                       // - íîìåð ñ÷¸ò÷èêà äëÿ 2-é ôóíêöèè 
        int width_2  = 24;                      // - øèðèíà òåêñòîâîãî áëîêà äëÿ 2-é ôóíêöèè
        int speed_2 = 1;                        // - ñêîðîñòü ïåðåëèñòûâàíèÿ äëÿ 2-é ôóíêöèè
        int total_lines = 0;                    // - âñåãî ëèíèé, êîòîðûå áóäóò âûâîäèòñÿ
        string[] stroka = new string[7];        // - ñòðîêè
        int procent = 0;
        int curentPage = 0;
        int totalPage = 0;
        // ============================== Ôóíêöèÿ ¹2 âûâîäèò òåêñò áëîêîì. =============================================
        // $dll(txt_manager, 2, "òåêñò", ñòðîê_âñåãî # íîìåð_òåêóùåé_ñòðîêè  # øèðèíà_áëîêà # ñêîðîñòü # òðàíñëèòåðàöèÿ(åñëè íóæíî) # òèï_ïðîêðóòêè
        // ïðèìåð èñïîëüçîâàíèÿ $dll(txt_manager, 2, Ïðèâåò Ìèð! ß ñòðîêà èç íåäð ýòîãî êîìïüþòåðà., 1#1#24#20#0)
        // $dll(txt_manager, 2, Ïðèâåò Ìèð! ß ñòðîêà èç íåäð ýòîãî êîìïüþòåðà., 1#1#24#20#Tr) - ñ òðàíñëèòåðàöèåé
        public string function2(string param1, string param2)
        {
            /* Îïèñàíèå sub_prm2[] */ /*       
             *       sub_prm2[0] = ÷èñëî ñòðîê
             *       sub_prm2[1] = íîìåð ñòðîêè
             *       sub_prm2[2] = øèðèíà òåêñòîâîãî áëîêà
             *       sub_prm2[3] = ñêîðîñòü ïåðåëèñòûâàíèÿ 
             *       sub_prm2[4] = Tr òðàíñëèòåðàöèÿ ðóññêîãî òåêñòà 
             *       sub_prm2[5] = Ln - ïîñòðî÷íàÿ ïðîêðóòêà, Sn - ïðêðóòêà çìåéêîé, áåç ïàðàìåòðîâ - ïîñòðàíè÷íî
             *       param2 = %  - âûâåñòè ïðîöåíò ïðî÷èòàííîãî òåêñòà
             *       param2 = tp - âûâåñòè ÷èñëî ñòðàíèö
             *       param2 = cp - âûâåñòè íîìåð òåêóùåé ñòðàíèöû   
             */
            string[] sub_prm2 = param2.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);// - ìàññèâ ñ ïàðàììåòðàìè
            int nmb_line;    // - íîìåð ñòðîêè  
            int full_page;   // - ìàêñèìàëüíîå ÷èñëî ñèìâîëâ íà ñòðàíèöå;
            int nul_char = 0;    // - ÷èñëî íåäîñòàþùèõ ñèìâîëîâ â êîíöå ñòðîêè äëÿ ðîâíîãî ñ÷¸òà ñòðàíèö
            
            if (param2 == "%")     { return procent.ToString(); }    // - âîçâðàùàåì ïðîöåíò ïðî÷èòàííîãî òåêñòà
            if (param2 == "tp")    { return totalPage.ToString(); }  // - âîçâðàùàåì ÷èñëî ñòðàíèö
            if (param2 == "cp")    { return curentPage.ToString(); } // - âîçâðàùàåì íîìåð òåêóùåé ñòðàíèöû
            if (param2 == "cp/tp") { return curentPage.ToString() + "/" + totalPage.ToString();}
            try
            {
                nmb_line = Convert.ToInt32(sub_prm2[1]);// - íîìåð ëèíèè
                
                if (sub_prm2[1] == "1")// - åñëè ïðèøëà ïåðâàÿ ñòðîêà, "ìóòèì" âåñü êîä
                {
                    total_lines = Convert.ToInt32(sub_prm2[0]);
                    number_2 = number_txt(param1); 
                    if (sub_prm2[4] == "Tr") { param1 = ASII.transliton(param1); } // - òðàíñëèòåðàöèÿ ðóññêèõ ñëîâ
                    width_2 = Convert.ToInt32(sub_prm2[2]);
                    speed_2 = Convert.ToInt32(sub_prm2[3]);
                    full_page = width_2 * total_lines;                        // - ìàêñèìàëüíîå ÷èñëî ñèìâîëâ íà ñòðàíèöå
                    if (param1.Length % full_page != 0) {
                        nul_char = full_page - (param1.Length % full_page); } // - ÷èñëî íåäîñòàþùèõ ñèìâîëîâ
                    for (int i = 0; i < nul_char; i++) { param1 += " "; }     // - ôîðìèðóåì ïóñòûå ñèìâîëû â êîíöå
                    lenght_text = param1.Length;                              // - äëèíà ñòðîêè 
                    procent = (counter[number_2] / (width_2 * total_lines) * 100) / (lenght_text / (width_2 * total_lines));
                    curentPage = (counter[number_2] / (width_2 * total_lines)) + 1;
                    totalPage = lenght_text / (width_2 * total_lines);
                    try 
                    {
                        if (sub_prm2[5] == "Ln") // - âêëþ÷àåì ïîñòðî÷íóþ ïðîêðóêó ïðè óñëîâèè, ÷òî sub_prm2[5] == "Ln" 
                        {
                            for (int i = 0; i < width_2 * total_lines; i++) { param1 = " " + param1 + " "; }
                            full_page = width_2;
                            lenght_text = param1.Length - (width_2 * total_lines) - nul_char;
                        }
                        if (sub_prm2[5] == "Sn") // - âêëþ÷àåì ïðîêðóòêó òåêñòà "çìåéêîé", åëñè sub_prm2[5] == "Sn" 
                        {
                            for (int i = 0; i < width_2 * total_lines; i++) { param1 = " " + param1 + " "; }
                            full_page = 1;
                            lenght_text = param1.Length - (width_2 * (total_lines - 1)) - nul_char;
                        }
                    }
                    catch { }
                    byte[] asciiBytes = Encoding.Default.GetBytes(param1);    // - ðàçáèâàåì òåêñò íà ìàññèâ ñ êîäàìè ñèìâîëîâ
                    
                    if (param1.Length < width_2) { width_2 = param1.Length; } // - åñëè äëèííà ñòðîêè ìåíüøå øèðèíû áëîêà çàìîðàæèâàåì ïåðåëèñòûâàíèå
                    if (old_speed_2[number_2] >= speed_2) { old_speed_2[number_2] = 0; counter[number_2] += full_page; } // - ðåãóëèðóåì ñêîðîñòü îáíîâëåíèÿ
                    if (counter[number_2] >= (lenght_text - width_2)) { counter[number_2] = 0; }   // - ñáðàñûâàåì ñ÷¸ò÷èê äëèíû ñòðîêè åñëè äîøëè äî êîíöà ñòðîêè

                    for (int l = 1; l <= total_lines; l++)
                    {
                        stroka[l] = "";
                        for (int i = (width_2 * (l - 1)) + counter[number_2]; i < (width_2 * l) + counter[number_2]; i++)// - âûâîä ñèìâîëîâ íà øèðèíó áëîêà
                        {
                            if (i >= asciiBytes.Length) {break;}                  // - âûõîäèì èç öèêëà åñëè i ïðåâûøàåò ðàçìåð ìàññèâà asciiBytes (òàêîå ïðîèñõîäèò ïðè ñìåíå øèðèíû áëîêà â ïðèëîæåíèè, ÷òî ïðèâîäèò ê îøèáêå â êîíöå òåêñòà)
                            asciiBytes[i] = tabl.ASII_table[asciiBytes[i]];       // - ïðîãîíêà ñèìâîëîâ ÷åðåç òàáëèöó ïîäìåíû
                            stroka[l] += "$Chr(" + asciiBytes[i].ToString() + ")";// - çàïîëíÿåì âûõîäíóþ ñòðîêó ñèìâîëàìè 
                        }
                    }
                }
            if (nmb_line == total_lines) { old_speed_2[number_2]++; } // - åñëè ïðèøëà ïîñëåäíÿÿ ñòðîêà îáíîâëÿåì ñ÷¸ò÷èê ñêîðîñòè è â ñëåäóùåì öèêëå âûâîäèì íîâóþ ñòðàíèöó
            return stroka[nmb_line];   // - âîçâðàùàåì òåêñò ïî íîìåðó ñòðîêè 
            }
            catch { return function1("Îøèáêà ïëàãèíà - txt_manager.dll! Íåâåðíàÿ çàïèñü - " + param2 + ".", "24#1#0"); }
        }

      
        // ======================= Ôóíêöèÿ ¹3 äåêîäèðîâàíèÿ ñèìâîëîâ â HTML ôîðìàòå ====================================
        //
        public string function3(string param1, string param2)
        {
            /*
           * Ëþòûé èçâðàò. Ðàäè îäíîé ðàäèîñòàíöèè - "Âàíÿ".
           * Íàçâàíèå òðåêîâ ïåðåäà¸òñÿ â HTML ôîðìàòå - (ê ïðèìåðó &#1040; -"À").
           * Ôóíêöèÿ èùåò è âîçâðàùàåò âñ¸ íîìåðàìè ñèìâîëîâ â ASII êîäèðîâêå. (ê ïðèìåðó "$Chr(192)" - "À";)
           */
            string html_char = "";// - html êîä ñèìâîëà
            string exit = "";     // - âûõîäíàÿ ñòðîêà
            int int_ = 0;         // - íîìåð ñèìâîëà â ASII êîäèðîâêå
            int flag = 0;         // - ôëàã èçâëå÷åíèÿ  html ñèìâîëà
            byte[] asciiBytes = Encoding.Default.GetBytes(param1);// - ìàññèâ ñèìâîëâî â ASII êîäèðîâêå
            for (int i = 0; i < asciiBytes.Length; i++) // - ïåðåáîð ìàññèâà
            {
                /* Çíà÷åíèÿ ôëàãà ïðè äåòåêòèðîâàíèè html ñèìâîëà
                 * ---------------- &#1040; --------------
                 * flag = 0 - html ñèìâîë íå íàéäåí
                 * flag = 1 - íàéäåí ñèìâîë - "&"
                 * flag = 2 - íàéäåí ñèìâîë - "#" ñðàçó ïîñëå "&", íàéäåí ñèìâîë â html êîäèðîâêå
                 */

                if (flag == 0) 
                {
                    if(asciiBytes[i] != '&') 
                    exit += "$Chr(" + Convert.ToInt32(asciiBytes[i]) + ")";
                }
                if (asciiBytes[i] != '#' && flag == 1)
                {
                    flag = 0;
                    exit += "$Chr(" + Convert.ToInt32('&') + ")";          // - äîáàâëÿåì ñèìâîë "&" åñëè îí íå èìååò îòíîøåíèå ê html ñèìâîëó
                    exit += "$Chr(" + Convert.ToInt32(asciiBytes[i]) + ")";// - äîàâëÿåì ïðî÷èå íå html ñèìâîëû
                }
                if (asciiBytes[i] == '&') { flag = 1; }              // - äåòåêòîð html ñèìâîëà
                if (asciiBytes[i] == '#' && flag == 1) { flag = 2; } //
                if (asciiBytes[i] == ';' && flag == 2) // - îáðàáîòêà ïîëó÷åííîãî html ñèìâîëà            
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
                    html_char += Convert.ToChar(asciiBytes[i]); // - ôîðìèðîâàíèå html ñèìâîëà
                }
            }
            return exit;
        }   

        
        int open_file_flag = 0; // - ôëàã ðàçîâîãî ïîêàçà ôàéëà "ïðî÷òè" 

        // ============================== Ôóíêöèÿ ¹4 äëÿ âûçîâà ñïðàâêè ==============================
        //
        public string function4(string param1, string param2)
        {
            Readmy_txt txt = new Readmy_txt();
            if (param1 == "about") { return function1("Ïëàãèí txt_manager V1.0 - ðàáîòà ñ òåêñòîì. Àâòîð - Ñàïóíîâ Ä.Â. email - sdv180@gmail.com. Êåìåðîâî 2022 ãîä.", "24#2#0"); }
            if (param1 == "help")
            {
                Encoding win1251 = Encoding.GetEncoding(1251);
                File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/txt_manager.txt", txt.file, win1251);
                if (open_file_flag == 0) { open_file_flag = 1; Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/txt_manager_ïðî÷òè.txt"); }
                return function1("×èòàé îïèñàíèå ê ïëàãèíó! Ôàéë -'txt_manager.txt', ñîõðàí¸í íà ðàáî÷åì ñòîëå.", "24#2#0");
            }
            return function1("Ñïðàâêà. Óçíàòü áîëüøå - óêàæèòå param1: 'about','help'.", "24#2#0");
        }


        // ============================== Ôóíêöèÿ ¹5 - Îáðåçàòü ñòðîêó ==============================
        //   $dll(txt_manager,5,Ïðèâåò,3#6) - "èâåò"
        //   $dll(txt_manager,5, Ïðèâåò,1#3) - "Ïðè"
        //
        public string function5(string param1, string param2)
        {
            /*   
             *   sub_prm2[0] =  íà÷àëî îáðåçêè
             *   sub_prm2[1] =  êîíåö
             */
            string[] sub_prm2 = param2.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);// - ìàññèâ ñ ïàðàììåòðàìè
            string exit = "";
            byte[] asciiBytes = Encoding.Default.GetBytes(param1);
            for (int i = 0; i < asciiBytes.Length; i++)
            {
                if(i >= Convert.ToInt32(sub_prm2[0]) - 1 && i <= Convert.ToInt32(sub_prm2[1]) - 1)
                exit += "$Chr(" + asciiBytes[i].ToString() + ")";
            }
            return exit;
        }

        // ============================== Ôóíêöèÿ ¹6 - Óäàëèòü ÷àñòü ñòðîêè ==============================
        //   $dll(txt_manager,5,Ïðèâåò,3#6) - "Ïðèò"
        //   $dll(txt_manager,5, Ïðèâåò,1#3) - "Ïèâåò"
        //
        public string function6(string param1, string param2)
        {
            /*   
             *   sub_prm2[0] =  ñ êàêîãî ñèìâîëà óäàëèòü ÷àñòü ñòðîêè
             *   sub_prm2[1] =  äî êàêîãî ñèìâîëà óäàëèòü ÷àñòü ñòðîêè
             */
            string[] sub_prm2 = param2.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);// - ìàññèâ ñ ïàðàììåòðàìè
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
            
            string[] sub_prm2 = param2.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);// - ìàññèâ ñ ïàðàììåòðàìè
            
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
