#ifndef TELRECINTERFACE_H
#define TELRECINTERFACE_H

#include "TelRecType.h"

/*API*/
#ifdef __cplusplus
extern "C"
{
#else
#include <stdbool.h>
#endif
    int TelRecAPI_Init();
    int TelRecAPI_Exit();
    int TelRecAPI_CheckDeviceID(char DeviceID[DeviceID_Length]);
    long TelRecAPI_CreateDevice(char DeviceID[DeviceID_Length]);
    int TelRecAPI_DeleteDevice(long Device);
    unsigned int TelRecAPI_StringToIPaddress(const char *Str);
    unsigned int TelRecAPI_StringToVersion(const char *Str);
    void TelRecAPI_IPaddressToString(unsigned int IPaddress, char OutString[16]);
    void TelRecAPI_VersionToString(unsigned int Version, char OutString[16]);
    /*Login Parameter*/
    int TelRecAPI_SetNetAddr(long Device, const char *IPaddress, unsigned short Port);
    int TelRecAPI_SetUserPassword(long Device, const char *UserName, int UserNameLength, const char *Password);
    /*Device Info*/
    const char *TelRecAPI_DeviceModel(long Device);
    const char *TelRecAPI_FirmwareVersion(long Device);
    int TelRecAPI_DeviceChannels(long Device);
    bool TelRecAPI_IsSupportPlayBack(long Device);
    /*Permission*/
    bool TelRecAPI_Permission_ManageUsers(long Device);
    bool TelRecAPI_Permission_ChangeSetting(long Device);
    bool TelRecAPI_Permission_Download(long Device);
    bool TelRecAPI_Permission_Delete(long Device);
    bool TelRecAPI_Permission_Monitor(long Device);
    bool TelRecAPI_Permission_Channel(long Device, int Channel);
    /*Device Status*/
    ConnectStatusType TelRecAPI_ConnectStatus(long Device);
    TelRec_StorageStatus *TelRecAPI_StorageStatus(long Device);
    TelRec_NetStatus *TelRecAPI_NetStatus(long Device);
    int TelRecAPI_LastPhoneStatus(long Device, int Channel);
    TelRec_ChannelStatus *TelRecAPI_ChannelStatus(long Device, int Channel);
    TelRec_OnlineUserList *TelRecAPI_OnlineUserList(long Device);
    bool TelRecAPI_CloudServerHasConnected(long Device);
    bool TelRecAPI_SimulateOffHookIsEnabled(long Device, int Channel);
    /*Device Setting*/
    TelRec_DateTime *TelRecAPI_DateTime(long Device);
    TelRec_PlayBackFiles *TelRecAPI_PlayBackFileList(long Device);
    TelRec_BaseSetting *TelRecAPI_BaseSetting(long Device);
    TelRec_ChannelSetting *TelRecAPI_ChannelSetting(long Device, int Channel);
    TelRec_KeyControlSetting *TelRecAPI_KeyControlSetting(long Device);
    TelRec_NetSetting *TelRecAPI_NetSetting(long Device);
    TelRec_SMDRSetting *TelRecAPI_SMDRSetting(long Device);
    TelRec_UserList *TelRecAPI_UserList(long Device);
    TelRec_RecordTimeSetting *TelRecAPI_RecordTimeSetting(long Device);
    TelRec_ScheduledRestartSetting *TelRecAPI_ScheduledRestartSetting(long Device);
    /*Device Operation*/
    int TelRecAPI_Login(long Device, bool RemoteLogin);
    int TelRecAPI_Logout(long Device);
    int TelRecAPI_GetStorageStatus(long Device);
    int TelRecAPI_GetNetStatus(long Device);
    int TelRecAPI_GetChannelVoltage(long Device, int Channel, int *Voltage);
    int TelRecAPI_GetCurrentRecordInfo(long Device, int Channel, CurrentRecordInfo *Info);
    int TelRecAPI_GetTime(long Device);
    int TelRecAPI_SetTime(long Device, TelRec_DateTime *NewDateTime);
    int TelRecAPI_GetPlayBackFileList(long Device);
    int TelRecAPI_GetBaseSetting(long Device);
    int TelRecAPI_SetBaseSetting(long Device, TelRec_BaseSetting *Setting);
    int TelRecAPI_GetChannelSetting(long Device, int Channel);
    int TelRecAPI_SetChannelSetting(long Device, int Channel, TelRec_ChannelSetting *Setting);
    int TelRecAPI_GetKeyControlSetting(long Device);
    int TelRecAPI_SetKeyControlSetting(long Device, TelRec_KeyControlSetting *Setting);
    int TelRecAPI_GetNetSetting(long Device);
    int TelRecAPI_SetNetSetting(long Device, TelRec_NetSetting *Setting);
    int TelRecAPI_GetSMDRSetting(long Device);
    int TelRecAPI_SetSMDRSetting(long Device, TelRec_SMDRSetting *Setting);
    int TelRecAPI_GetUserList(long Device);
    int TelRecAPI_AddUser(long Device, TelRec_UserInfo *User, const char *Password);
    int TelRecAPI_EditUser(long Device, TelRec_UserInfo *User, const char *Password);
    int TelRecAPI_DeleteUser(long Device, TelRec_UserInfo *User);
    int TelRecAPI_RemoveFile(long Device, const char *FilePath);
    int TelRecAPI_GetLatestRecordTime(long Device, unsigned char *Year, unsigned char *Month, unsigned char *Day);
    int TelRecAPI_GetEarliestRecordTime(long Device, unsigned char *Year, unsigned char *Month, unsigned char *Day);
    int TelRecAPI_GetDayListFromMonthDir(long Device, unsigned char Year, unsigned char Month, unsigned char DayArray[32]);
    int TelRecAPI_EditRecordNotes(long Device, unsigned short ItemOffset, unsigned char Year, unsigned char Month, unsigned char Day, unsigned char Channel, const char *Notes);
    int TelRecAPI_DeleteRecord(long Device, RecordDeleteItem *Item);
    int TelRecAPI_Dial(long Device, unsigned char Channel, const char *PhoneNum, int PhoneNumLength);
    int TelRecAPI_GetSMDR(long Device, int *Length, char **Data);
    int TelRecAPI_Reboot(long Device);
    int TelRecAPI_GetRecordTimeSetting(long Device);
    int TelRecAPI_SetRecordTimeSetting(long Device, TelRec_RecordTimeSetting *Setting);
    int TelRecAPI_GetScheduledRestartSetting(long Device);
    int TelRecAPI_SetScheduledRestartSetting(long Device, TelRec_ScheduledRestartSetting *Setting);
    int TelRecAPI_PlayAudioFile(long Device, int Channel, const char *FileName);
    int TelRecAPI_OffHook(long Device, int Channel);

#ifdef __cplusplus
    int TelRecAPI_SearchDevice(EventCallBack CallBack);
    int TelRecAPI_CreateHeartbeatThread(long Device, EventCallBack CallBack);
    int TelRecAPI_UploadFile(long Device, const char *SrcFilePath, const char *UploadDir, const char *UploadFileName, EventCallBack CallBack);
    int TelRecAPI_DownloadFile(long Device, const char *FilePath, EventCallBack CallBack);

    int TelRecAPI_GetNewVersionInfo(long Device, EventCallBack CallBack);
    int TelRecAPI_DownloadFirmware(long Device, const char *FileName, EventCallBack CallBack);
    int TelRecAPI_CheckFirmware(long Device, const char *FirmwarePath);
    int TelRecAPI_GetClientVersionInfo(EventCallBack CallBack);
#endif

    int TelRecAPI_C_SearchDevice(EventCallBack_C CallBack);
    int TelRecAPI_C_CreateHeartbeatThread(long Device, EventCallBack_C CallBack);
    int TelRecAPI_C_UploadFile(long Device, const char *SrcFilePath, const char *UploadDir, const char *UploadFileName, EventCallBack_C CallBack);
    int TelRecAPI_C_DownloadFile(long Device, const char *FilePath, EventCallBack_C CallBack);

#ifdef __cplusplus
}
#endif

#endif
