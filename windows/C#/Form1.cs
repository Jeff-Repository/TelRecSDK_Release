using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace CSharpDemo
{
    public partial class Form1 : Form
    {
        IntPtr Device = IntPtr.Zero;
        Thread SearchThread;
        bool UploadFileThreadWorking = false;
        bool DownloadFileThreadWorking = false;

        void HeartbeatThreadCallBack(TelRecInterface.TelRecEventType Event, IntPtr Device, int Channel, int Arg)
        {
            switch (Event)
            {
                case TelRecInterface.TelRecEventType.ConnectStatusChanged:
                    {
                        Console.WriteLine("Device connect status changed : " + TelRecInterface.ConnectStatus(Device));
                        //MessageBox.Show("Device connect status changed : " + TelRecInterface.ConnectStatus(Device));
                        break;
                    }
                case TelRecInterface.TelRecEventType.StorageStatusChanged:
                    {
                        TelRecInterface.TelRecStorageStatus Status = TelRecInterface.StorageStatus(Device);
                        Console.WriteLine("Device storage status changed : " + Status.Status);
                        if (Status.Status == TelRecInterface.StorageStatusType.Normal)
                        {
                            Console.WriteLine("Capacity : " + Status.Capacity + "MB");
                            Console.WriteLine("Free : " + Status.Free + "MB");
                        }
                        break;
                    }
                case TelRecInterface.TelRecEventType.CloudServerStatusChanged:
                    {
                        bool CloudServerHasConnected = TelRecInterface.CloudServerHasConnected(Device);
                        Console.WriteLine("Device cloud connect status changed : " + CloudServerHasConnected);
                        break;
                    }
                case TelRecInterface.TelRecEventType.OnlineUserListChanged:
                    {
                        List<TelRecInterface.TelRecOnlineUser> OnlineUserList = TelRecInterface.OnlineUserList(Device);
                        for (int i = 0; i < OnlineUserList.Count; i++)
                        {
                            Console.WriteLine("Online user : " + OnlineUserList[i].UserName + ", IP : " + OnlineUserList[i].IP);
                            //...
                        }
                        break;
                    }
                case TelRecInterface.TelRecEventType.ChannelStatusChanged:
                    {
                        TelRecInterface.TelRecChannelStatus Status = TelRecInterface.ChannelStatus(Device, Channel);
                        Console.WriteLine("Channel : " + Channel + ", Status : " + Status.PhoneStatus);
                        if (Status.PhoneNum.Length > 0)
                            Console.WriteLine("Channel : " + Channel + ", PhoneNumber : " + Status.PhoneNum);
                        //...
                        break;
                    }
                case TelRecInterface.TelRecEventType.ChannelPlayBackChanged:
                    {
                        Console.WriteLine("Channel : " + Channel + ", PlayBack is " + ((Arg > 0) ? "enabled" : "disabled"));
                        break;
                    }
                case TelRecInterface.TelRecEventType.ChannelTalkTimeChanged:
                    {
                        Console.WriteLine("Channel : " + Channel + ", Talk time : " + TelRecInterface.TalkTimeToString(Arg));
                        break;
                    }
                case TelRecInterface.TelRecEventType.ChannelMonitorChanged:
                    {
                        Console.WriteLine("Channel : " + Channel + ", Monitor is " + ((Arg > 0) ? "enabled" : "disabled"));
                        break;
                    }
            }
        }

        void SearchThreadFunction()
        {
            this.Invoke((EventHandler)delegate
            {
                LogTextBox.Clear();
            });
            TelRecInterface.SearchDevice((TelRecInterface.TelRecFoundDeviceInfo Info, int SearchProgress) =>
            {
                if (Info != null)
                {
                    this.Invoke((EventHandler)delegate
                    {
                        LogTextBox.AppendText("Found Device\r\n");
                        LogTextBox.AppendText("Device ID : " + Info.DeviceID + "\r\n");
                        LogTextBox.AppendText("Model : " + Info.Model + "\r\n");
                        LogTextBox.AppendText("FirmwareVersion : " + Info.Version + "\r\n");
                        LogTextBox.AppendText("IPaddress : " + Info.IPaddress + "\r\n");
                        LogTextBox.AppendText("NetPort : " + Info.NetPort + "\r\n");
                        LogTextBox.AppendText("Channels : " + Info.Channels + "\r\n");
                        /*Input info to TextBox*/
                        DeviceIDTextBox.Text = Info.DeviceID;
                        IPaddressTextBox.Text = Info.IPaddress;
                        PortTextBox.Text = Info.NetPort.ToString();
                    });
                }
                else
                {
                    Console.WriteLine("Search Progress : " + SearchProgress);
                }
            });
        }

        bool CheckDevice()
        {
            if (Device == IntPtr.Zero)
            {
                LogTextBox.AppendText("Device cannot be empty\r\n");
                return false;
            }
            return true;
        }

        public Form1()
        {
            InitializeComponent();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            TelRecInterface.Init();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (Device == IntPtr.Zero)
            {
                TelRecInterface.DeleteDevice(Device);
            }
            TelRecInterface.Exit();
        }

        private void SearchDeviceButton_Click(object sender, EventArgs e)
        {
            if (SearchThread != null)
            {
                if (SearchThread.ThreadState == ThreadState.Running)
                    return;
            }
            SearchThread = new Thread(SearchThreadFunction);
            SearchThread.Start();
        }

        private void CreateDeviceButton_Click(object sender, EventArgs e)
        {
            LogTextBox.Clear();
            if (Device != IntPtr.Zero)
            {
                LogTextBox.AppendText("The device has already been created\r\n");
                return;
            }
            string DeviceID = DeviceIDTextBox.Text;
            if (string.IsNullOrEmpty(DeviceID))
            {
                LogTextBox.AppendText("Device ID cannot be empty\r\n");
                return;
            }
            Device = TelRecInterface.CreateDevice(DeviceID);
            if (Device != IntPtr.Zero)
            {
                LogTextBox.AppendText("Device created successfully" + "\r\n");
                LogTextBox.AppendText("Device Model : " + TelRecInterface.DeviceModel(Device) + "\r\n");
                LogTextBox.AppendText("Device Channels : " + TelRecInterface.DeviceChannels(Device) + "\r\n");
            }
            else
            {
                LogTextBox.AppendText("Device created failed, Please check the device ID\r\n");
            }
        }

        private void RemoteLoginCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            IPaddressTextBox.Enabled = !RemoteLoginCheckBox.Checked;
            PortTextBox.Enabled = !RemoteLoginCheckBox.Checked;
        }

        private void LoginButton_Click(object sender, EventArgs e)
        {
            string UserName, Password, IPaddress = "0";
            int Port = 0;
            bool RemoteLogin = RemoteLoginCheckBox.Checked;
            LogTextBox.Clear();
            if (!CheckDevice())
                return;
            /*Check UserName*/
            UserName = UserNameTextBox.Text;
            if (string.IsNullOrEmpty(UserName))
            {
                LogTextBox.AppendText("UserName Invalid\r\n");
                return;
            }
            if(UserName.Length > TelRecInterface.UserNameLengthMax)
            {
                LogTextBox.AppendText("UserName Invalid\r\n");
                return;
            }
            /*Check Password*/
            Password = PasswordTextBox.Text;
            if (string.IsNullOrEmpty(Password))
            {
                LogTextBox.AppendText("Password Invalid\r\n");
                return;
            }
            if (!RemoteLogin)
            {
                /*Check IPaddress*/
                IPaddress = IPaddressTextBox.Text;
                if (string.IsNullOrEmpty(IPaddress))
                {
                    LogTextBox.AppendText("IPaddress Invalid\r\n");
                    return;
                }
                /*Check Port*/
                if (!int.TryParse(PortTextBox.Text, out Port))
                {
                    LogTextBox.AppendText("Port Invalid\r\n");
                    return;
                }
                if (Port > 65535)
                {
                    LogTextBox.AppendText("Port Invalid\r\n");
                    return;
                }
            }
            TelRecInterface.TelRecErrno Errno = TelRecInterface.Login(Device, IPaddress, (ushort)Port, UserName, Password, RemoteLoginCheckBox.Checked);
            if (Errno == TelRecInterface.TelRecErrno.Succeed)
                LogTextBox.AppendText("Login successfully\r\n");
            else
                LogTextBox.AppendText("Login failed, Errno : " + Errno + "\r\n");
        }

        private void LogoutButton_Click(object sender, EventArgs e)
        {
            if (!CheckDevice())
                return;
            TelRecInterface.Logout(Device);
            LogTextBox.AppendText("Logout successfully\r\n");
        }

        private void CreateHeartbeatThreadButton_Click(object sender, EventArgs e)
        {
            if (!CheckDevice())
                return;
            TelRecInterface.CreateHeartbeatThread(Device, HeartbeatThreadCallBack);
        }

        private void GetStorageStatusButton_Click(object sender, EventArgs e)
        {
            LogTextBox.Clear();
            if (!CheckDevice())
                return;
            /*Update status cache from Device*/
            TelRecInterface.TelRecErrno Errno = TelRecInterface.GetStorageStatus(Device);
            if (Errno == TelRecInterface.TelRecErrno.Succeed)
            {
                /*Get status cache*/
                TelRecInterface.TelRecStorageStatus Status = TelRecInterface.StorageStatus(Device);
                LogTextBox.AppendText("Get device storage status successfully\r\n");
                LogTextBox.AppendText("Status : " + Status.Status + "\r\n");
                LogTextBox.AppendText("Capacity : " + Status.Capacity + " MB\r\n");
                LogTextBox.AppendText("Free : " + Status.Free + " MB\r\n");
            }
            else
                LogTextBox.AppendText("Get device storage status failed, Errno : " + Errno + "\r\n");
        }

        private void GetNetStatusButton_Click(object sender, EventArgs e)
        {
            LogTextBox.Clear();
            if (!CheckDevice())
                return;
            /*Update status cache from Device*/
            TelRecInterface.TelRecErrno Errno = TelRecInterface.GetNetStatus(Device);
            if (Errno == TelRecInterface.TelRecErrno.Succeed)
            {
                /*Get status cache*/
                TelRecInterface.TelRecNetStatus Status = TelRecInterface.NetStatus(Device);
                LogTextBox.AppendText("Get device net status successfully\r\n");
                LogTextBox.AppendText("IPaddress : " + Status.IPaddress + "\r\n");
            }
            else
                LogTextBox.AppendText("Get device net status failed, Errno : " + Errno + "\r\n");
        }

        private void GetTimeButton_Click(object sender, EventArgs e)
        {
            LogTextBox.Clear();
            if (!CheckDevice())
                return;
            TelRecInterface.TelRecErrno Errno = TelRecInterface.GetTime(Device);
            if (Errno == TelRecInterface.TelRecErrno.Succeed)
            {
                TelRecInterface.TelRecDateTime Time = TelRecInterface.DateTime(Device);
                LogTextBox.AppendText("Get device time snapshoot successfully\r\n");
                LogTextBox.AppendText("Device Time : " + Time.ToString() + "\r\n");
            }
            else
                LogTextBox.AppendText("Get device time snapshoot failed, Errno : " + Errno + "\r\n");
        }

        private void GetPlayBackFileListButton_Click(object sender, EventArgs e)
        {
            LogTextBox.Clear();
            if (!CheckDevice())
                return;
            TelRecInterface.TelRecErrno Errno = TelRecInterface.GetPlayBackFileList(Device);
            if (Errno == TelRecInterface.TelRecErrno.Succeed)
            {
                List<string> Files = TelRecInterface.PlayBackFiles(Device);
                LogTextBox.AppendText("Get playback file list successfully\r\n");
                for (int i = 0; i < Files.Count; i++)
                {
                    LogTextBox.AppendText(Files[i] + "\r\n");
                }
            }
            else
                LogTextBox.AppendText("Get playback file list failed, Errno : " + Errno + "\r\n");
        }

        private void GetBaseSettingButton_Click(object sender, EventArgs e)
        {
            LogTextBox.Clear();
            if (!CheckDevice())
                return;
            TelRecInterface.TelRecErrno Errno = TelRecInterface.GetBaseSetting(Device);
            if (Errno == TelRecInterface.TelRecErrno.Succeed)
            {
                TelRecInterface.TelRecBaseSetting Setting = TelRecInterface.BaseSetting(Device);
                LogTextBox.AppendText("Get device base setting successfully\r\n");
                LogTextBox.AppendText("RecordTimeMin : " + Setting.RecordTimeMin + "\r\n");
                LogTextBox.AppendText("RecordTimeMax : " + Setting.RecordTimeMax + "\r\n");
                LogTextBox.AppendText("RingEndTime : " + Setting.RingEndTime + "\r\n");
                LogTextBox.AppendText("PhoneNumLengthMin : " + Setting.PhoneNumLengthMin + "\r\n");
                LogTextBox.AppendText("CircuitOutNum : " + Setting.CircuitOutNum + "\r\n");
                LogTextBox.AppendText("VoiceSensitivity : " + Setting.VoiceSensitivity + "\r\n");
                LogTextBox.AppendText("SaveMissedCall : " + Setting.SaveMissedCall + "\r\n");
                LogTextBox.AppendText("ReserveSpace : " + Setting.ReserveSpace + "\r\n");
                LogTextBox.AppendText("LoopUseStorage : " + Setting.LoopUseStorage + "\r\n");
                LogTextBox.AppendText("DialWaitOffHookTime : " + Setting.DialWaitOffHookTime + "\r\n");
                LogTextBox.AppendText("KeyRecordStart : " + Setting.KeyRecordStart + "\r\n");
                LogTextBox.AppendText("KeyRecordEnd : " + Setting.KeyRecordEnd + "\r\n");
            }
            else
                LogTextBox.AppendText("Get device base setting failed, Errno : " + Errno + "\r\n");
        }

        private void GetChannelSettingButton_Click(object sender, EventArgs e)
        {
            /*Get Channel 0*/
            int Channel = 0;
            LogTextBox.Clear();
            if (!CheckDevice())
                return;
            TelRecInterface.TelRecErrno Errno = TelRecInterface.GetChannelSetting(Device, Channel);
            if (Errno == TelRecInterface.TelRecErrno.Succeed)
            {
                TelRecInterface.TelRecChannelSetting Setting = TelRecInterface.ChannelSetting(Device, Channel);
                LogTextBox.AppendText("Get device channel setting successfully\r\n");
                LogTextBox.AppendText("EnableLostWarning : " + Setting.EnableLostWarning + "\r\n");
                LogTextBox.AppendText("EnableAnnouncement : " + Setting.EnableAnnouncement + "\r\n");
                LogTextBox.AppendText("SaveAnnouncementToRecord : " + Setting.SaveAnnouncementToRecord + "\r\n");
                LogTextBox.AppendText("RecordCondition : " + Setting.RecordCondition + "\r\n");
                LogTextBox.AppendText("......\r\n");
            }
            else
                LogTextBox.AppendText("Get device channel setting failed, Errno : " + Errno + "\r\n");
        }

        private void GetKeyControlSettingButton_Click(object sender, EventArgs e)
        {
            LogTextBox.Clear();
            if (!CheckDevice())
                return;
            TelRecInterface.TelRecErrno Errno = TelRecInterface.GetKeyControlSetting(Device);
            if (Errno == TelRecInterface.TelRecErrno.Succeed)
            {
                TelRecInterface.TelRecKeyControlSetting Setting = TelRecInterface.KeyControlSetting(Device);
                LogTextBox.AppendText("Get device key control setting successfully\r\n");
                LogTextBox.AppendText("PlayBackStartKey[0].Enable : " + Setting.PlayBackStartKey[0].Enable + "\r\n");
                LogTextBox.AppendText("PlayBackStartKey[0].Key : " + Setting.PlayBackStartKey[0].Key + "\r\n");
                LogTextBox.AppendText("PlayBackStartKey[0].FileName : " + Setting.PlayBackStartKey[0].FileName + "\r\n");
                LogTextBox.AppendText("......\r\n");
            }
            else
                LogTextBox.AppendText("Get device key control setting failed, Errno : " + Errno + "\r\n");
        }

        private void GetNetSettingButton_Click(object sender, EventArgs e)
        {
            LogTextBox.Clear();
            if (!CheckDevice())
                return;
            TelRecInterface.TelRecErrno Errno = TelRecInterface.GetNetSetting(Device);
            if (Errno == TelRecInterface.TelRecErrno.Succeed)
            {
                TelRecInterface.TelRecNetSetting Setting = TelRecInterface.NetSetting(Device);
                LogTextBox.AppendText("Get device net setting successfully\r\n");
                LogTextBox.AppendText("IP : " + Setting.IP + "\r\n");
                LogTextBox.AppendText("Mask : " + Setting.Mask + "\r\n");
                LogTextBox.AppendText("Gateway : " + Setting.Gateway + "\r\n");
                LogTextBox.AppendText("MAC : " + Setting.MAC + "\r\n");
                LogTextBox.AppendText("Port : " + Setting.Port + "\r\n");
                LogTextBox.AppendText("Mode : " + Setting.Mode + "\r\n");
                LogTextBox.AppendText("EnableCloud : " + Setting.EnableCloud + "\r\n");
            }
            else
                LogTextBox.AppendText("Get device net setting failed, Errno : " + Errno + "\r\n");
        }

        private void GetSMDRSettingButton_Click(object sender, EventArgs e)
        {
            LogTextBox.Clear();
            if (!CheckDevice())
                return;
            TelRecInterface.TelRecErrno Errno = TelRecInterface.GetSMDRSetting(Device);
            if (Errno == TelRecInterface.TelRecErrno.Succeed)
            {
                int[] Baudrate = { 1200, 2400, 4800, 9600, 14400, 19200, 38400, 56000, 57600, 115200 };
                int[] DataBit = { 8, 9 };
                float[] StopBit = { 1.0f, 1.5f, 2.0f };
                TelRecInterface.TelRecSMDRSetting Setting = TelRecInterface.SMDRSetting(Device);
                LogTextBox.AppendText("Get device SMDR setting successfully\r\n");
                LogTextBox.AppendText("Enable : " + Setting.Enable + "\r\n");
                LogTextBox.AppendText("CheckTime : " + Setting.CheckTime + "\r\n");
                LogTextBox.AppendText("......\r\n");
                LogTextBox.AppendText("BaudrateOption : " + Baudrate[Setting.BaudrateOption] + "\r\n");
                LogTextBox.AppendText("DataBitOption : " + DataBit[Setting.DataBitOption] + "\r\n");
                LogTextBox.AppendText("StopBitOption : " + StopBit[Setting.StopBitOption] + "\r\n");
                LogTextBox.AppendText("......\r\n");
            }
            else
                LogTextBox.AppendText("Get device SMDR setting failed, Errno : " + Errno + "\r\n");
        }
        
        private void GetRecordTimeSettingButton_Click(object sender, EventArgs e)
        {
            LogTextBox.Clear();
            if (!CheckDevice())
                return;
            TelRecInterface.TelRecErrno Errno = TelRecInterface.GetRecordTimeSetting(Device);
            if (Errno == TelRecInterface.TelRecErrno.Succeed)
            {
                string[] ModeText = { "Whitelist", "Blacklist", "Timing" };
                string[] WeekText = { "Sun", "Mon", "Tue", "Wed", "Thu", "Fri", "Sat" };
                TelRecInterface.TelRecRecordTimeSetting Setting = TelRecInterface.RecordTimeSetting(Device);
                LogTextBox.AppendText("Get device record time setting successfully\r\n");
                LogTextBox.AppendText("Enable : " + Setting.Enable + "\r\n");
                LogTextBox.AppendText("Mode : " + ModeText[Setting.Mode] + "\r\n");
                for(int week = 0; week < 7; week++)
                {
                    int offset = 3 * week;
                    LogTextBox.AppendText(WeekText[week] + " Time1 Enable : " + Setting.Quantum[offset].Enable + "\r\n");
                    LogTextBox.AppendText(WeekText[week] + " Time2 Enable : " + Setting.Quantum[offset + 1].Enable + "\r\n");
                    LogTextBox.AppendText(WeekText[week] + " Time3 Enable : " + Setting.Quantum[offset + 2].Enable + "\r\n");
                    LogTextBox.AppendText(WeekText[week] + " Time1 Start Hour : " + Setting.Quantum[offset].StartHour + "\r\n");
                    LogTextBox.AppendText(WeekText[week] + " Time1 End Hour : " + Setting.Quantum[offset].EndHour + "\r\n");
                    LogTextBox.AppendText("......\r\n");
                }
            }
            else
                LogTextBox.AppendText("Get device record time setting failed, Errno : " + Errno + "\r\n");
        }

        private void GetUserListButton_Click(object sender, EventArgs e)
        {
            LogTextBox.Clear();
            if (!CheckDevice())
                return;
            TelRecInterface.TelRecErrno Errno = TelRecInterface.GetUserList(Device);
            if (Errno == TelRecInterface.TelRecErrno.Succeed)
            {
                List<TelRecInterface.TelRecUserInfo> UserList = TelRecInterface.UserList(Device);
                LogTextBox.AppendText("Get user list successfully\r\n");
                foreach (TelRecInterface.TelRecUserInfo User in UserList)
                {
                    LogTextBox.AppendText("User Name :" + User.UserName + "\r\n");
                    //...
                }
            }
            else
                LogTextBox.AppendText("Get user list failed, Errno : " + Errno + "\r\n");
        }

        private void OffHookTestButton_Click(object sender, EventArgs e)
        {
            TelRecInterface.OffHook(Device, 0);
        }

        private void UploadFileButton_Click(object sender, EventArgs e)
        {
            if (UploadFileThreadWorking)
                return;
            if (!CheckDevice())
                return;
            TelRecInterface.TelRecErrno Errno;
            UploadFileThreadWorking = true;
            OpenFileDialog OFD = new OpenFileDialog();
            if (OFD.ShowDialog() == DialogResult.OK)
            {
                LogTextBox.Clear();
                LogTextBox.AppendText("Uploading...\r\n");
                Thread UploadFileThread = new Thread(() =>
                {
                    Errno = TelRecInterface.UploadFile(Device, OFD.FileName, "/PlayBackFiles", null, (IntPtr Device, int Progressa) =>
                    {
                        this.Invoke((EventHandler)delegate
                        {
                            ProgressBar.Value = Progressa;
                        });
                        return false;//return true will stop upload
                    });
                    UploadFileThreadWorking = false;
                    this.Invoke((EventHandler)delegate
                    {
                        if (Errno == TelRecInterface.TelRecErrno.Succeed)
                            LogTextBox.AppendText("Upload successfully\r\n");
                        else
                            LogTextBox.AppendText("Upload failed, Errno : " + Errno + "\r\n");
                    });
                });
                UploadFileThread.Start();
            }
        }

        private void DownloadFileButton_Click(object sender, EventArgs e)
        {
            /*Download small text files to memory and display text*/
            /*File path is /1.txt in device storage*/
            if (DownloadFileThreadWorking)
                return;
            if (!CheckDevice())
                return;
            TelRecInterface.TelRecErrno Errno;
            int DownloadCount = 0, FileSize = 0;
            int Progress;
            byte[] FileBuffer = null;
            DownloadFileThreadWorking = true;
            LogTextBox.Clear();
            Thread DownloadFileThread = new Thread(() =>
            {
                Errno = TelRecInterface.DownloadFile(Device, "/1.txt", (IntPtr Device, byte[] Data, int Length) =>
                {
                    if (Data == null)//GotDataSize
                    {
                        FileSize = Length;
                        FileBuffer = new byte[FileSize];
                    }
                    else
                    {
                        Array.Copy(Data, 0, FileBuffer, DownloadCount, Length);
                        DownloadCount += Length;
                        Progress = DownloadCount * 100 / FileSize;
                        if (Progress != ProgressBar.Value)
                        {
                            this.Invoke((EventHandler)delegate
                            {
                                ProgressBar.Value = Progress;
                            });
                        }
                    }
                    return false;//return true will stop download
                });
                DownloadFileThreadWorking = false;
                this.Invoke((EventHandler)delegate
                {
                    if (Errno == TelRecInterface.TelRecErrno.Succeed)
                    {
                        LogTextBox.AppendText("Download successfully\r\n");
                        LogTextBox.AppendText("1.txt:\r\n\r\n");
                        LogTextBox.AppendText(Encoding.UTF8.GetString(FileBuffer) + "\r\n");
                    }
                    else
                        LogTextBox.AppendText("Download failed, Errno : " + Errno + "\r\n");
                });
            });
            DownloadFileThread.Start();
        }

        private void GetDayListFromMonthDirButton_Click(object sender, EventArgs e)
        {
            byte[] Array = new byte[32];
            TelRecInterface.GetDayListFromMonthDir(Device,18, 12, Array);
            for (int i = 1; i < 32; i++)
            {
                if(Array[i] > 0)
                    Console.WriteLine(i.ToString());
            }
        }

        private void GetRecordInfoButton_Click(object sender, EventArgs e)
        {
            TelRecInterface.TelRecCurrentRecordInfo Info;
            TelRecInterface.TelRecErrno Errno = TelRecInterface.GetCurrentRecordInfo(Device, 0, out Info);
            if (Errno == TelRecInterface.TelRecErrno.Succeed)
            {
                LogTextBox.AppendText("Get Channel 0 Current Record Info successfully\r\n");
                LogTextBox.AppendText("Year: " + Info.Year + "\r\n");
                LogTextBox.AppendText("Month: " + Info.Month + "\r\n");
                LogTextBox.AppendText("Day: " + Info.Day + "\r\n");
                LogTextBox.AppendText("Hour: " + Info.Hour + "\r\n");
                LogTextBox.AppendText("Minutes: " + Info.Minutes + "\r\n");
                LogTextBox.AppendText("Seconds: " + Info.Seconds + "\r\n");
                LogTextBox.AppendText("RecordID: " + Info.RecordID + "\r\n");
                LogTextBox.AppendText("RecordType: " + Info.RecordType + "\r\n");
                LogTextBox.AppendText("PhoneNum: " + Info.PhoneNum + "\r\n");
            }
            else
            {
                LogTextBox.AppendText("Get Channel 0 Current Record Info, Errno : " + Errno + "\r\n");
            }
        }
    }
}
