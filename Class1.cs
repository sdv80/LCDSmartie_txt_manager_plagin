
//
// dot net plugins are supported in LCD Smartie 5.3 beta 3 and above.
//
// ====  ������ "txt_manager" - ������ ������ � �������. ====
// ����� - ������� �.�. 
// �������� 2022 �.
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
            for (int i = 0; i < old_speed.Length; i++){ old_speed[i] = 100; }// - ������ ������ ������� ��� ������ ����� 100, ��� ����������� ����� � ������� - if (old_speed[number] >= speed)
        }
        static int element = 50; // - ����� ����������� ��������� (����� �������� ��� ���� �������)
        ASII tabl = new ASII();                                 // - ������� ������� ��������
        ASII_transliteration ASII = new ASII_transliteration(); // - ����� ��� ���������������
        string old_txt = ""; // - ������� ������
        int old_number = 0;  // - ������� �����
        private int number_txt(string param1)
        {
            if (old_txt == param1) { return old_number; }// - ���� ������ ������� ������ ���������� ������ ����� ��������
            int frch = 0;
            foreach (string id_txt in in_txt)
            {
                if (id_txt == param1) {  break; }   // - ���� ����� �������
                if (id_txt == null) { in_txt[frch] = param1;  counter[frch] = 0; break; }// - ���� ������ �� �����, �� �������� � �� ������ ��������� ����� � �������
                frch++;
                if (frch == in_txt.Length) { for (int i = 0; i < in_txt.Length; i++) { in_txt[i] = null; } }  // - ���� ��������� ������ ����� ������ ������� ������ ������
            }
            old_txt = param1;
            old_number = frch;
            return frch;
        }

        // ======================= ������� ��� ���������� ������ �������� � ������ ==================================
        //
        private string param1_processing(string param1, string param2)
        {
            string[] sub_prm2 = param2.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);// - ������ � ������������
            string beginning = "";// - ������ � ������
            string end = "";      // - ������ � �����
            width = Convert.ToInt32(sub_prm2[0]);
            //------------------------- ���������� ������ �������� � ������ � � ����� -----------------------------------
            for (int i = 0; i < width; i++)
            {
                if (i % 2 == 0) { beginning += " "; } // - ��������� ������ ������� � ������, ��� ���-�� ������ �������� ��������� � ��������
                end += " ";                          // - ��������� ������ ������� � �����. ����� ������� ����� ������ �����.
            }
            if (sub_prm2[2] == "R" || sub_prm2[2] == "TrR") { end = beginning = " "; } // - ������� ������ ������� � ������ � � ����� ���� ������� ������
            if (param1.Length <= width) { width = param1.Length; end = beginning = ""; } // "���������" ������ ���� ������ ������ �����
            param1 = beginning + param1 + end;             // - ��������� ������ ������� � ������ � � �����
            //-----------------------------------------------------------------------------------------------------------
            return param1;
        }

        int[] old_speed = new int[element];    // - ���� �������� ��� 1-�� �������
        string[] exit   = new string[element]; // - �������� �����
        string[] in_txt = new string[element]; // - ����� �� �����, ��� ���������� ����������� ������ ��������
        int[] counter   = new int[element];    // - ������ � ���������� 
        int lenght_text = 0;                   // - ������ ������ (����������� ��������)
        int revers_flag = 0;                   // - ���� ������� 
        int width  = 24;                       // - ������ ���������� �����

        // ============================ ������� �1 ������� ����� ������� �������. ======================================
        // $dll(txt_manager, 1, "�����", #  ������_����� # �������� # ������ #.)
        public string function1(string param1, string param2)
        {
            /*
             *       sub_prm2[0] = ������ ���������� �����
             *       sub_prm2[1] = �������� ������
             *       sub_prm2[2] = "R"   - ��������� ������ ��������� ������ (������ �������������� �� ����� ���� ��� ������ �����)                     
             *                     "Tr"  - �������������� �������� ������
             *                     "TrR" - �������������� �������� ������ � ��������
             *            param2 = "tc" - ������ ������
             *                     "��" - ������� ��������� ������
             *                      "%" - ������� ������� ������������ ������
             */
            string[] sub_prm2 = param2.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);// - ������ � ������������
            int number;  // - ����� ��������  
            int speed;  // - �������� ������� ������
            int char_poz;  // - ���������� ����� ������� � ������
            int temp;  // - ��� ��������
            try
            {
                number = number_txt(param1);// - ������ ������ ��������                            
                if (param2 == "help") { return function4("help", "");}
                if (param2 == "about") { return function4("about", ""); }
                if (param2 == "%")  { temp = counter[number] * 100 / (lenght_text - width); return temp.ToString(); } // - ���������� ������� ������������ ������
                if (param2 == "tc") { return lenght_text.ToString(); }     // - ���������� ������ ������
                if (param2 == "cc") { return counter[number].ToString(); } // - ���������� ������� ��������� ������

                if (sub_prm2[2] == "Tr" || sub_prm2[2] == "TrR") { param1 = ASII.transliton(param1); }  // - �������������� ������� ����

                width  = Convert.ToInt32(sub_prm2[0]);                     // - ��������� ������ �����                                    
                speed  = Convert.ToInt32(sub_prm2[1]);                     // - ��������� �������� ���������
                param1 = param1_processing(param1, param2);                // - ���������� ������ �������� � ������ � � �����
               
                if (param2 != "%") // - ���������� �����
                {
                    byte[] asciiBytes = Encoding.Default.GetBytes(param1); // - ��������� ����� �� ������ � ������ ��������
                    lenght_text = asciiBytes.Length;                       // - ������ ������ 
                    if (counter[number] > (lenght_text - width)) { counter[number] = 0; revers_flag++; if (revers_flag == 2) { revers_flag = 0; } }// - ���������� ������� ������ ������ � ���� ������� ���� ����� �� ����� ������
                    if (old_speed[number] >= speed)                        // - ���������� �������� ������
                    {
                        exit[number] = "";                                 // - ������� �������� ������ ����� ����������� 
                        for (int i = 0; i < width; i++)                    // - ����� �������� �� ������ �����
                        {
                            char_poz = i + counter[number];                // - ��������� ������� � ������� ��� �������� � ����
                            if ((sub_prm2[2] == "R" || sub_prm2[2] == "TrR") && revers_flag == 1){ char_poz = i + (lenght_text - counter[number] - width); }// - ��������� ������� � ������� ��� �������� � ����� (������)
                            asciiBytes[char_poz] = tabl.ASII_table[asciiBytes[char_poz]];    // - �������� �������� ����� ������� �������                             
                            exit[number] += "$Chr(" + asciiBytes[char_poz].ToString() + ")"; // - ��������� �������� ������ ��������� 
                        }
                        old_speed[number] = 0; // - ����� ����� ��������
                        counter[number]++;     // - ������� �������
                        return exit[number];   // - ���������� ����� ������� ��������
                    }
                }
                old_speed[number]++;
                return exit[number]; // - ���������� ����� ������� ��������
              
            }
            catch {return function1("������ ������� - txt_manager.dll! �������� ������ - " + param2 + ".", "24#1#0");}
        }
        //================================================================================================================
        //
        
        int[] old_speed_2 = new int[element];   // - ���� �������� ��� 2-�� �������   
        int number_2 = 0;                       // - ����� �������� ��� 2-� ������� 
        int width_2  = 24;                      // - ������ ���������� ����� ��� 2-� �������
        int speed_2 = 1;                        // - �������� �������������� ��� 2-� �������
        int total_lines = 0;                    // - ����� �����, ������� ����� ���������
        string[] stroka = new string[7];        // - ������
        int procent = 0;
        int curentPage = 0;
        int totalPage = 0;
        // ============================== ������� �2 ������� ����� ������. =============================================
        // $dll(txt_manager, 2, "�����", �����_����� # �����_�������_������  # ������_����� # �������� # ��������������(���� �����) # ���_���������
        // ������ ������������� $dll(txt_manager, 2, ������ ���! � ������ �� ���� ����� ����������., 1#1#24#20#0)
        // $dll(txt_manager, 2, ������ ���! � ������ �� ���� ����� ����������., 1#1#24#20#Tr) - � ���������������
        public string function2(string param1, string param2)
        {
            /* �������� sub_prm2[] */ /*       
             *       sub_prm2[0] = ����� �����
             *       sub_prm2[1] = ����� ������
             *       sub_prm2[2] = ������ ���������� �����
             *       sub_prm2[3] = �������� �������������� 
             *       sub_prm2[4] = Tr �������������� �������� ������ 
             *       sub_prm2[5] = Ln - ���������� ���������, Sn - �������� �������, ��� ���������� - �����������
             *       param2 = %  - ������� ������� ������������ ������
             *       param2 = tp - ������� ����� �������
             *       param2 = cp - ������� ����� ������� ��������   
             */
            string[] sub_prm2 = param2.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);// - ������ � ������������
            int nmb_line;    // - ����� ������  
            int full_page;   // - ������������ ����� ������� �� ��������;
            int nul_char = 0;    // - ����� ����������� �������� � ����� ������ ��� ������� ����� �������
            
            if (param2 == "%")     { return procent.ToString(); }    // - ���������� ������� ������������ ������
            if (param2 == "tp")    { return totalPage.ToString(); }  // - ���������� ����� �������
            if (param2 == "cp")    { return curentPage.ToString(); } // - ���������� ����� ������� ��������
            if (param2 == "cp/tp") { return curentPage.ToString() + "/" + totalPage.ToString();}
            try
            {
                nmb_line = Convert.ToInt32(sub_prm2[1]);// - ����� �����
                
                if (sub_prm2[1] == "1")// - ���� ������ ������ ������, "�����" ���� ���
                {
                    total_lines = Convert.ToInt32(sub_prm2[0]);
                    number_2 = number_txt(param1); 
                    if (sub_prm2[4] == "Tr") { param1 = ASII.transliton(param1); } // - �������������� ������� ����
                    width_2 = Convert.ToInt32(sub_prm2[2]);
                    speed_2 = Convert.ToInt32(sub_prm2[3]);
                    full_page = width_2 * total_lines;                        // - ������������ ����� ������� �� ��������
                    if (param1.Length % full_page != 0) {
                        nul_char = full_page - (param1.Length % full_page); } // - ����� ����������� ��������
                    for (int i = 0; i < nul_char; i++) { param1 += " "; }     // - ��������� ������ ������� � �����
                    lenght_text = param1.Length;                              // - ����� ������ 
                    procent = (counter[number_2] / (width_2 * total_lines) * 100) / (lenght_text / (width_2 * total_lines));
                    curentPage = (counter[number_2] / (width_2 * total_lines)) + 1;
                    totalPage = lenght_text / (width_2 * total_lines);
                    try 
                    {
                        if (sub_prm2[5] == "Ln") // - �������� ���������� �������� ��� �������, ��� sub_prm2[5] == "Ln" 
                        {
                            for (int i = 0; i < width_2 * total_lines; i++) { param1 = " " + param1 + " "; }
                            full_page = width_2;
                            lenght_text = param1.Length - (width_2 * total_lines) - nul_char;
                        }
                        if (sub_prm2[5] == "Sn") // - �������� ��������� ������ "�������", ���� sub_prm2[5] == "Sn" 
                        {
                            for (int i = 0; i < width_2 * total_lines; i++) { param1 = " " + param1 + " "; }
                            full_page = 1;
                            lenght_text = param1.Length - (width_2 * (total_lines - 1)) - nul_char;
                        }
                    }
                    catch { }
                    byte[] asciiBytes = Encoding.Default.GetBytes(param1);    // - ��������� ����� �� ������ � ������ ��������
                    
                    if (param1.Length < width_2) { width_2 = param1.Length; } // - ���� ������ ������ ������ ������ ����� ������������ ��������������
                    if (old_speed_2[number_2] >= speed_2) { old_speed_2[number_2] = 0; counter[number_2] += full_page; } // - ���������� �������� ����������
                    if (counter[number_2] >= (lenght_text - width_2)) { counter[number_2] = 0; }   // - ���������� ������� ����� ������ ���� ����� �� ����� ������

                    for (int l = 1; l <= total_lines; l++)
                    {
                        stroka[l] = "";
                        for (int i = (width_2 * (l - 1)) + counter[number_2]; i < (width_2 * l) + counter[number_2]; i++)// - ����� �������� �� ������ �����
                        {
                            if (i >= asciiBytes.Length) {break;}                  // - ������� �� ����� ���� i ��������� ������ ������� asciiBytes (����� ���������� ��� ����� ������ ����� � ����������, ��� �������� � ������ � ����� ������)
                            asciiBytes[i] = tabl.ASII_table[asciiBytes[i]];       // - �������� �������� ����� ������� �������
                            stroka[l] += "$Chr(" + asciiBytes[i].ToString() + ")";// - ��������� �������� ������ ��������� 
                        }
                    }
                }
            if (nmb_line == total_lines) { old_speed_2[number_2]++; } // - ���� ������ ��������� ������ ��������� ������� �������� � � �������� ����� ������� ����� ��������
            return stroka[nmb_line];   // - ���������� ����� �� ������ ������ 
            }
            catch { return function1("������ ������� - txt_manager.dll! �������� ������ - " + param2 + ".", "24#1#0"); }
        }

      
        // ======================= ������� �3 ������������� �������� � HTML ������� ====================================
        //
        public string function3(string param1, string param2)
        {
            /*
           * ����� ������. ���� ����� ������������ - "����".
           * �������� ������ ��������� � HTML ������� - (� ������� &#1040; -"�").
           * ������� ���� � ���������� �� �������� �������� � ASII ���������. (� ������� "$Chr(192)" - "�";)
           */
            string html_char = "";// - html ��� �������
            string exit = "";     // - �������� ������
            int int_ = 0;         // - ����� ������� � ASII ���������
            int flag = 0;         // - ���� ����������  html �������
            byte[] asciiBytes = Encoding.Default.GetBytes(param1);// - ������ �������� � ASII ���������
            for (int i = 0; i < asciiBytes.Length; i++) // - ������� �������
            {
                /* �������� ����� ��� �������������� html �������
                 * ---------------- &#1040; --------------
                 * flag = 0 - html ������ �� ������
                 * flag = 1 - ������ ������ - "&"
                 * flag = 2 - ������ ������ - "#" ����� ����� "&", ������ ������ � html ���������
                 */

                if (flag == 0) 
                {
                    if(asciiBytes[i] != '&') 
                    exit += "$Chr(" + Convert.ToInt32(asciiBytes[i]) + ")";
                }
                if (asciiBytes[i] != '#' && flag == 1)
                {
                    flag = 0;
                    exit += "$Chr(" + Convert.ToInt32('&') + ")";          // - ��������� ������ "&" ���� �� �� ����� ��������� � html �������
                    exit += "$Chr(" + Convert.ToInt32(asciiBytes[i]) + ")";// - �������� ������ �� html �������
                }
                if (asciiBytes[i] == '&') { flag = 1; }              // - �������� html �������
                if (asciiBytes[i] == '#' && flag == 1) { flag = 2; } //
                if (asciiBytes[i] == ';' && flag == 2) // - ��������� ����������� html �������            
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
                    html_char += Convert.ToChar(asciiBytes[i]); // - ������������ html �������
                }
            }
            return exit;
        }   

        
        int open_file_flag = 0; // - ���� �������� ������ ����� "������" 

        // ============================== ������� �4 ��� ������ ������� ==============================
        //
        public string function4(string param1, string param2)
        {
            Readmy_txt txt = new Readmy_txt();
            if (param1 == "about") { return function1("������ txt_manager V1.0 - ������ � �������. ����� - ������� �.�. email - sdv180@gmail.com. �������� 2022 ���.", "24#2#0"); }
            if (param1 == "help")
            {
                Encoding win1251 = Encoding.GetEncoding(1251);
                File.WriteAllText(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/txt_manager.txt", txt.file, win1251);
                if (open_file_flag == 0) { open_file_flag = 1; Process.Start(Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory) + "/txt_manager_������.txt"); }
                return function1("����� �������� � �������! ���� -'txt_manager.txt', ������� �� ������� �����.", "24#2#0");
            }
            return function1("�������. ������ ������ - ������� param1: 'about','help'.", "24#2#0");
        }


        // ============================== ������� �5 - �������� ������ ==============================
        //   $dll(txt_manager,5,������,3#6) - "����"
        //   $dll(txt_manager,5, ������,1#3) - "���"
        //
        public string function5(string param1, string param2)
        {
            /*   
             *   sub_prm2[0] =  ������ �������
             *   sub_prm2[1] =  �����
             */
            string[] sub_prm2 = param2.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);// - ������ � ������������
            string exit = "";
            byte[] asciiBytes = Encoding.Default.GetBytes(param1);
            for (int i = 0; i < asciiBytes.Length; i++)
            {
                if(i >= Convert.ToInt32(sub_prm2[0]) - 1 && i <= Convert.ToInt32(sub_prm2[1]) - 1)
                exit += "$Chr(" + asciiBytes[i].ToString() + ")";
            }
            return exit;
        }

        // ============================== ������� �6 - ������� ����� ������ ==============================
        //   $dll(txt_manager,5,������,3#6) - "����"
        //   $dll(txt_manager,5, ������,1#3) - "�����"
        //
        public string function6(string param1, string param2)
        {
            /*   
             *   sub_prm2[0] =  � ������ ������� ������� ����� ������
             *   sub_prm2[1] =  �� ������ ������� ������� ����� ������
             */
            string[] sub_prm2 = param2.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);// - ������ � ������������
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
            
            string[] sub_prm2 = param2.Split(new char[] { '#' }, StringSplitOptions.RemoveEmptyEntries);// - ������ � ������������
            
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
