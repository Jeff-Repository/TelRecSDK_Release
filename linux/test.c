#include <stdio.h>
#include <string.h>
#include "TelRecInterface.h"

static bool DeviceFound = false;
static TelRec_FoundDeviceInfo DeviceInfo;

static int SearchDeviceCallBack(TelRec_EventType Event, long Device, unsigned char *Data, int Length)
{
    
    char DeviceID[DeviceID_Length + 1];
    if(Event == UpdateProgress)
    {
        printf("Search Progress:%d\n", Length);
    }
    else if(Event == FoundDevice)
    {
        if(DeviceFound)
        {
            /*测试只处理一个设备*/
            return 0;
        }
    
        TelRec_FoundDeviceInfo *info = (TelRec_FoundDeviceInfo *)Data;
        memcpy(DeviceID, info->DeviceID, DeviceID_Length);
        DeviceID[DeviceID_Length] = '\0';
        printf("----------Device Found----------\n");
        printf("ID:%s\n", DeviceID);
        printf("Model:%s\n", info->Model);
        printf("Channels:%d\n", info->Channels);
        printf("Version:%s\n", info->Version);
        printf("IPaddress:%s\n", info->IPaddress);
        printf("NetPort:%d\n", info->NetPort);
        printf("--------------------------------\n");

        memcpy(&DeviceInfo, info, sizeof(TelRec_FoundDeviceInfo));
        DeviceFound = true;
    }
}

static int HeartbeatCallBack(TelRec_EventType Event, long Device, unsigned char *Data, int Length)
{
    int channel;
    TelRec_ChannelStatus *Status;
    char PhoneNum[PhoneNumLengthMax + 1];
    const char *StatusText[] = { "掉线", "挂机", "摘机", "振铃", "来电", "去电", "自动应答", "声控录音有效", "声控录音无效" };
    switch(Event)
    {
        case ConnectStatusChanged:
        case StorageStatusChanged:
        case CloudServerStatusChanged:
        case OnlineUserListChanged:
            break;
        case ChannelStatusChanged:
        {
            channel = (int)(long)Data;
            Status = TelRecAPI_ChannelStatus(Device, channel);
            if(Status->PhoneStatus < PhoneStatusTypeMax)
            {
                printf("Channel:%d, Status:%s\n", channel, StatusText[Status->PhoneStatus]);
            }
            if(Status->PhoneNumLength > 0)
            {
                memcpy(PhoneNum, Status->PhoneNum, Status->PhoneNumLength);
                PhoneNum[Status->PhoneNumLength] = '\0';
                printf("Channel:%d, Call Number:%s\n", channel, PhoneNum);
            }
            break;
        }
        case ChannelPlayBackChanged:
        case ChannelTalkTimeChanged:
        case ChannelMonitorChanged:
            break;
    }
    return 0;
}

int main()
{
    long device = 0;

    TelRecAPI_Init();

    TelRecAPI_C_SearchDevice(SearchDeviceCallBack);

    if(DeviceFound)
    {
        device = TelRecAPI_CreateDevice(DeviceInfo.DeviceID);
        TelRecAPI_SetNetAddr(device, DeviceInfo.IPaddress, DeviceInfo.NetPort);
        TelRecAPI_SetUserPassword(device, "admin", sizeof("admin") - 1, "admin");
        if(TelRecAPI_Login(device, false) == 0)
        {
            printf("Login Succeed\n");
            if(TelRecAPI_C_CreateHeartbeatThread(device, HeartbeatCallBack) == 0)
            {
                printf("Create Heartbeat Thread Succeed\n");
            }
        }
    }

    printf("Press any key to exit\n");
    getchar();

    if(device != 0)
    {
        TelRecAPI_Logout(device);
        TelRecAPI_DeleteDevice(device);
    }

    TelRecAPI_Exit();
}
