using Risk.net.Utilities.Objects;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Risk.net.Utilities.Functions
{
    public class Mail
    {
        static string smtpKullanici;
        static string smtpParola;
        static string smtpAdres;
        static string smtpGuvenli;
        static string smtpBase64;
        static int smtpPort;
        static SmtpClient smtpClient;

        static Mail()
        {
            MailInit();
        }

        private static void MailInit()
        {
            smtpKullanici = Arac.ConfigOku("SMTP:Kullanici");
            smtpParola = Arac.ConfigOku("SMTP:Parola");
            smtpAdres = Arac.ConfigOku("SMTP:Adres");
            smtpGuvenli = Arac.ConfigOku("SMTP:Guvenli");
            smtpBase64 = Arac.ConfigOku("SMTP:Base64");
            smtpPort = 0;

            if (!string.IsNullOrEmpty(smtpAdres) && smtpAdres.IndexOf(":") > -1)
            {
                smtpPort = Arac.ConvertToInt(smtpAdres.Substring(smtpAdres.IndexOf(":") + 1), 0);
                smtpAdres = smtpAdres.Substring(0, smtpAdres.IndexOf(":"));
            }

            if (smtpPort == 0)
                smtpClient = new SmtpClient(smtpAdres);
            else
                smtpClient = new SmtpClient(smtpAdres, smtpPort);

            if (Arac.ConvertToInt(smtpGuvenli, 0) > 0)
                smtpClient.EnableSsl = true;


            if (!String.IsNullOrEmpty(smtpKullanici))
                smtpClient.Credentials = new System.Net.NetworkCredential(smtpKullanici, smtpParola);

            if (Arac.ConvertToInt(smtpBase64, 0) > 0)
                smtpClient.UseDefaultCredentials = false;

            smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
        }

        public static string MailAt(string from, string to, string konu, string bilgi)
        {
            return MailAtThread(from, to, "", "", konu, bilgi, false, false, null);
        }

        public static string MailAt(string from, string to, string konu, string bilgi, bool html, bool replyVar, List<object> ekDosyaIsimleri)
        {
            return MailAtThread(from, to, "", "", konu, bilgi, html, replyVar, ekDosyaIsimleri);
        }

        public static string MailAt(string from, string to, string cc, string bcc, string konu, string bilgi, bool html, bool replyVar, List<object> ekDosyaIsimleri)
        {
            return MailAtThread(from, to, cc, bcc, konu, bilgi, html, replyVar, ekDosyaIsimleri);
        }

        private static string MailAtHazirla(string from, string to, string cc, string bcc, string konu, string bilgi, bool html, bool replyVar, List<object> ekDosyaIsimleri)
        {
            if (string.IsNullOrWhiteSpace(from))
                from = Arac.ConfigOku("SMTP:Gonderen");

            if (string.IsNullOrWhiteSpace(from) && !string.IsNullOrWhiteSpace(smtpKullanici) && smtpKullanici.Contains("@"))
                from = smtpKullanici;

            if (string.IsNullOrWhiteSpace(from))
                return "Mail gönderebilmek için web.config dosyasında, SMTPAdres, SMTPKullanici, SMTPParola, SMTPGuvenli, SMTPBase64 ve SMTPGonderen değerlerini doldurunuz ";

            string donusDeger = "";
            string mail = to;
            if (mail.IndexOf('@') != -1)
            {
                string[] mailler = mail.Split(';');

                for (int i = 0; i < mailler.Length; i++)
                {
                    if (!string.IsNullOrEmpty(mailler[i]) && Arac.EPostaDogrula(mailler[i]) && !string.IsNullOrEmpty(from))
                    {
                        donusDeger = MailAtIc(from, mailler[i], cc, bcc, konu, bilgi, html, replyVar, ekDosyaIsimleri);
                    }
                }
            }
            return donusDeger;
        }

        private static string MailAtIc(string from, string to, string cc, string bcc, string konu, string bilgi, bool html, bool replyVar, List<object> ekDosyaIsimleri)
        {
            if (string.IsNullOrEmpty(smtpAdres))//Bazen smtp adresi boş oluyor. Bu sebeble tekrar init et
                MailInit();

            if (string.IsNullOrEmpty(smtpAdres))//Bazen smtp adresi boş oluyor. Bu sebeble tekrar init et
                return "Mail gönderebilmek için web.config dosyasında, SMTPAdres, SMTPKullanici, SMTPParola, SMTPGuvenli ve SMTPBase64 değerlerini doldurunuz ";

            string donusDeger = "";

            if (string.IsNullOrEmpty(from))
                from = Arac.ConfigOku("SMTP:Gonderen");

            System.Net.Mail.MailMessage mess = new System.Net.Mail.MailMessage();

            string fromAdi = "";
            if (from.IndexOf('|') > -1)
            {
                string[] fler = from.Split('|');
                if (fler.Length > 0) from = fler[0];
                if (fler.Length > 1) fromAdi = fler[1];
            }

            mess.From = new MailAddress(from, fromAdi);
            mess.Sender = new MailAddress(from, fromAdi);

            if (to != null) to = to.Replace(" ", "");
            if (cc != null) cc = cc.Replace(" ", "");
            if (bcc != null) bcc = bcc.Replace(" ", "");

            int say = 0;
            string hata = MailAdressEkle(mess.To, to, ref say);
            if (!string.IsNullOrWhiteSpace(hata)) return hata;
            hata = MailAdressEkle(mess.CC, cc, ref say);
            if (!string.IsNullOrWhiteSpace(hata)) return hata;
            hata = MailAdressEkle(mess.Bcc, bcc, ref say);
            if (!string.IsNullOrWhiteSpace(hata)) return hata;
            if (say == 0)
                return "Alıcısı olmayan mesaj gönderilmeye çalışılıyor!";
            mess.Subject = konu;
            mess.Body = bilgi;

            mess.BodyEncoding = System.Text.Encoding.GetEncoding("utf-8");
            mess.Priority = System.Net.Mail.MailPriority.Normal;

            if (html)
                mess.IsBodyHtml = true;

            string tmpYol = "";
            if (ekDosyaIsimleri != null && ekDosyaIsimleri.Count > 0)
            {
                System.Net.Mail.Attachment attach = null;

                for (int i = 0; i < ekDosyaIsimleri.Count; i++)
                {
                    string dosyaAd;
                    if (ekDosyaIsimleri[i].GetType() == typeof(string))
                        dosyaAd = (string)ekDosyaIsimleri[i];
                    else
                        dosyaAd = (((EPostaEki)ekDosyaIsimleri[i]).ad);

                    if (string.IsNullOrEmpty(dosyaAd)) continue;

                    if (ekDosyaIsimleri[i].GetType() == typeof(string))
                    {
                        attach = new System.Net.Mail.Attachment(dosyaAd);
                    }
                    else
                    {
                        EPostaEki ek = (EPostaEki)ekDosyaIsimleri[i];
                        if (ek.ek != null)
                        {
                            if (tmpYol == "")
                            {
                                tmpYol = Path.Combine(Path.GetTempPath(), Path.GetRandomFileName());
                                Directory.CreateDirectory(tmpYol);
                            }
                            string tmpDosya = Path.Combine(tmpYol, DosyaAdDegistir(dosyaAd)); //Dosya adında istenmeyen karakterleri temizleyip, geçici dosya adı üretiliyor.
                            File.WriteAllBytes(tmpDosya, ((EPostaEki)ekDosyaIsimleri[i]).ek);
                            attach = new System.Net.Mail.Attachment(tmpDosya);
                        }
                        else
                        {
                            attach = new System.Net.Mail.Attachment(ek.dosyaAd);
                        }
                        attach.Name = ek.ad;
                    }

                    mess.Attachments.Add(attach);
                }
            }

            if (replyVar)
                mess.Headers.Add("Return-Path", from);

            donusDeger = "";
            try
            {
                lock (smtpAdres)
                {
                    ServicePointManager.ServerCertificateValidationCallback = new System.Net.Security.RemoteCertificateValidationCallback(MyCertValidationCb);
                    smtpClient.Send(mess);
                }

                donusDeger = "1";
                string dosyaAdi = "epostaGonder" + DateTime.Now.Year.ToString() + "" + DateTime.Now.Month.ToString("00") + "Gidenler.txt";
                Arac.HataStrYaz(dosyaAdi, "to:" + mess.To + ";cc:" + cc + ";bcc:" + bcc + "-> Konu:" + mess.Subject);
            }
            catch (Exception ex)
            {
                string hataIc = ex.Message;
                if (ex.InnerException != null && !string.IsNullOrWhiteSpace(ex.InnerException.Message))
                {
                    hataIc += " (" + ex.InnerException.Message + ")";
                }
                donusDeger = "Mail gönderilemedi: " + hataIc + " bilgiler=> sunucu:" + smtpClient.Host + " Email Adresi:" + " to:" + mess.To + ";cc:" + cc + ";bcc:" + bcc;
                string dosyaAdi = "epostaGonder" + DateTime.Now.Year.ToString() + "" + DateTime.Now.Month.ToString("00") + "Hata.txt";
                Arac.HataStrYaz(dosyaAdi, "Gönderilemedi> donusDeger:" + donusDeger + "-smtpAdres:" + smtpAdres + "-smtpKullanici:" + smtpKullanici);
            }

            mess.Dispose();

            try
            {
                if (!string.IsNullOrEmpty(tmpYol))
                    Directory.Delete(Path.GetDirectoryName(tmpYol), true);
            }
            catch { Arac.HataStrYaz("Posta Gönderme ek klasörü silme hatası" + tmpYol); }


            return donusDeger;
        }

        private static string MailAtThread(string from, string to, string cc, string bcc, string konu, string bilgi, bool html, bool replyVar, List<object> ekDosyaIsimleri)
        {
            try
            {
                //to kısmında gelen maillerde aynı mail geçiyorsa elemek için
                //****************************************************************
                string[] kisiler = to.Split(';');
                for (int j = 0; j < kisiler.Length; j++)
                {
                    if (kisiler[j] != "")
                    {
                        for (int i = j + 1; i < kisiler.Length; i++)
                        {
                            if (kisiler[i] == kisiler[j]) kisiler[i] = "";
                        }
                    }
                }
                to = "";
                for (int i = 0; i < kisiler.Length; i++)
                {
                    if (kisiler[i] == "") continue;
                    if (to != "") to += ";";
                    to += kisiler[i];
                }
                //****************************************************************

                ParameterizedThreadStart paraThread = new ParameterizedThreadStart(MailAtThread);
                Thread th = new Thread(paraThread);
                th.Name = "MailAtThread";
                Object obj = new object[] { from, to, cc, bcc, konu, bilgi, html, replyVar, ekDosyaIsimleri };
                th.Start(obj);
                return "1";
            }
            catch (Exception ex)
            {
                Arac.HataStrYaz("{7895D27E-C6C2-440C-99D3-77155CE531BB} - Mail Gönderme Hatası; Kimden: " + from + ", Kime: " + to + ", Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
            return "0";
        }

        private static void MailAtThread(object obj)
        {
            try
            {
                Object[] liste = (Object[])obj;

                string from = (string)liste[0];
                string to = (string)liste[1];
                string cc = (string)liste[2];
                string bcc = (string)liste[3];
                string konu = (string)liste[4];
                string bilgi = (string)liste[5];
                bool html = (bool)liste[6];
                bool replyVar = (bool)liste[7];
                List<object> ekDosyaIsimleri = (List<object>)liste[8];

                Mail.MailAtHazirla(from, to, cc, bcc, konu, bilgi, html, replyVar, ekDosyaIsimleri);

            }
            catch (ThreadAbortException ex)
            {
                Arac.HataStrYaz("{8AAB495F-9CDB-42D6-8D64-76DD022B97E2} - MailAtThread1 Gönderme Hatası,  Message: " + ex.Message + " # StackTrace: " + ex.StackTrace);
            }
        }

        private static string MailAdressEkle(MailAddressCollection mailAddressCollection, string cc, ref int say)
        {
            string[] ccler = cc.Split(';');
            foreach (string gc in ccler)
            {
                string to = gc;
                string toAdi = "";

                if (to.IndexOf('|') > -1)
                {
                    string[] fler = to.Split('|');
                    if (fler.Length > 0) to = fler[0];
                    if (fler.Length > 1) toAdi = fler[1];
                }

                if (!string.IsNullOrEmpty(to))
                {
                    if (Arac.EPostaDogrula(to))
                    {
                        mailAddressCollection.Add(new MailAddress(to, toAdi));
                        say++;
                    }
                    else
                        return "Hatalı mail adresi:" + to;
                }
            }
            return "";
        }

        private static string DosyaAdDegistir(string p)
        {
            string[] degistir = { "/", "\\", ":", "?", "\"", "*", "<", ">", "|" };
            foreach (string dg in degistir)
            {
                p = p.Replace(dg, "");
            }
            return p;
        }

        private static bool MyCertValidationCb(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }
    }
}
