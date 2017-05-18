using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace ControllerUpgrade
{
  class Program
  {
    static void Main(string[] args)
    {
      string end = "_E_N_D";
      byte[] b_e = System.Text.ASCIIEncoding.ASCII.GetBytes(end);
      TcpClient client = new TcpClient("172.22.32.19",10255);
      byte[] b_r = new byte[256];
      write_mdm(client);
      Console.WriteLine("");
      write_sam(client);
      
      client.GetStream().Write(b_e, 0, b_e.Length);
      client.GetStream().Read(b_r, 0, 256);
      Thread.Sleep(2000);
      client.Close();

    }

    static void write_sam(TcpClient client)
    {
      string up_d20 = @"D:\Soft\DOCSRV_REP\SerialPortResolve\sams\sam-d20\Debug\sam-mdm-dev.bin";
      FileInfo fi_d = new FileInfo(up_d20);
      byte[] b_d = new byte[fi_d.Length];
      FileStream fs = new FileStream(up_d20, FileMode.Open);
      fs.Read(b_d, 0, b_d.Length);
      fs.Close();
      string sam = "_SAM_";
      byte[] b_s = System.Text.ASCIIEncoding.ASCII.GetBytes(sam);
      int pos = 0;
      int slen = 512;
      client.GetStream().Write(b_s, 0, b_s.Length);
      byte[] b_r = new byte[256];
      int full_len = b_d.Length;
      while (true)
      {
        if (pos + slen > full_len)
        {
          slen = (int)full_len - pos;
        }
        client.GetStream().Write(b_d, pos, slen);
        client.GetStream().Read(b_r, 0, 256);
        int per = ((pos) * 100) / full_len;
        pos += slen;
        Console.Write("\r Send SAM: " + per.ToString() + "%");// BitConverter.ToInt32(b_r, 0).ToString());
        if (pos == full_len)
          break;
      }
    }

    static void write_mdm(TcpClient client)
    {
      string up_m66 = @"D:\Soft\DOCSRV_REP\Quectel\Quectel\build\gcc\APPGS3MDM32A01_Upgrade_Package.bin";
      FileInfo fi_m = new FileInfo(up_m66);
      byte[] b_m = new byte[fi_m.Length];
      int full_len = (int)b_m.Length;
      string mdm = "_MDM_";
      byte[] b_s = System.Text.ASCIIEncoding.ASCII.GetBytes(mdm);
      FileStream fs = new FileStream(up_m66, FileMode.Open);
      fs.Read(b_m, 0, b_m.Length);
      fs.Close();
      int pos = 0;
      int slen = 512;
      client.GetStream().Write(b_s, 0, b_s.Length);
      byte[] b_r = new byte[256];
      while (true)
      {
        if (pos + slen > full_len)
        {
          slen = (int)full_len - pos;
        }
        client.GetStream().Write(b_m, pos, slen);
        client.GetStream().Read(b_r, 0, 256);
        int per = ((pos) * 100) / full_len;
        pos += slen;
        Console.Write("\r Send SAM: " + per.ToString() + "%");// BitConverter.ToInt32(b_r, 0).ToString());
        if (pos == full_len)
          break;
      }
    }
  }
}
  