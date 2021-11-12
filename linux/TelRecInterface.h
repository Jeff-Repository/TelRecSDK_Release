#ifndef TELRECINTERFACE_H
#define TELRECINTERFACE_H

#include <TelRecType.h>

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
    intptr_t TelRecAPI_CreateDevice(char DeviceID[DeviceID_Length]);
    int TelRecAPI_DeleteDevice(intptr_t Device);
    unsigned int TelRecAPI_StringToIPaddress(const char *Str);
    unsigned int TelRecAPI_StringToVersion(const char *Str);
    void TelRecAPI_IPaddressToString(unsigned int IPaddress, char OutString[16]);
    void TelRecAPI_VersionToString(unsigned int Version, char OutString[16]);
    /*Login Parameter*/
    int TelRecAPI_SetNetAddr(intptr_t Device, const char *IPaddress, unsigned short Port);
    int TelRecAPI_SetUserPassword(intptr_t Device, const char *UserName, int UserNameLength, const char *Password);
    /*Device Info*/
    const char *TelRecAPI_DeviceID(intptr_t Device);
    const char *TelRecAPI_DeviceModel(intptr_t Device);
    const char *TelRecAPI_FirmwareVersion(intptr_t Device);
    int TelRecAPI_DeviceChannels(intptr_t Device);
    bool TelRecAPI_IsSupportPlayBack(intptr_t Device);
    /*Permission*/
    bool TelRecAPI_Permission_ManageUsers(intptr_t Device);
    bool TelRecAPI_Permission_ChangeSetting(intptr_t Device);
    bool TelRecAPI_Permission_Download(intptr_t Device);
    bool TelRecAPI_Permission_Delete(intptr_t Device);
    bool TelRecAPI_Permission_Monitor(intptr_t Device);
    bool TelRecAPI_Permission_Channel(intptr_t Device, int Channel);
    /*Device Status*/
    ConnectStatusType TelRecAPI_ConnectStatus(intptr_t Device);
    TelRec_StorageStatus *TelRecAPI_StorageStatus(intptr_t Device);
    TelRec_NetStatus *TelRecAPI_NetStatus(intptr_t Device);
    int TelRecAPI_LastPhoneStatus(intptr_t Device, int Channel);
    TelRec_ChannelStatus *TelRecAPI_ChannelStatus(intptr_t Device, int Channel);
    TelRec_OnlineUserList *TelRecAPI_OnlineUserList(intptr_t Device);
    bool TelRecAPI_CloudServerHasConnected(intptr_t Device);
    bool TelRecAPI_SimulateOffHookIsEnabled(intptr_t Device, int Channel);
    /*Device Setting*/
    TelRec_DateTime *TelRecAPI_DateTime(intptr_t Device);
    TelRec_PlayBackFiles *TelRecAPI_PlayBackFileList(intptr_t Device);
    TelRec_BaseSetting *TelRecAPI_BaseSetting(intptr_t Device);
    TelRec_ChannelSetting *TelRecAPI_ChannelSetting(intptr_t Device, int Channel);
    TelRec_KeyControlSetting *TelRecAPI_KeyControlSetting(intptr_t Device);
    TelRec_NetSetting *TelRecAPI_NetSetting(intptr_t Device);
    TelRec_SMDRSetting *TelRecAPI_SMDRSetting(intptr_t Device);
    TelRec_UserList *TelRecAPI_UserList(intptr_t Device);
    TelRec_RecordTimeSetting *TelRecAPI_RecordTimeSetting(intptr_t Device);
    TelRec_ScheduledRestartSetting *TelRecAPI_ScheduledRestartSetting(intptr_t Device);
    /*Device Operation*/
    int TelRecAPI_Login(intptr_t Device, bool RemoteLogin);
    int TelRecAPI_Logout(intptr_t Device);
    int TelRecAPI_GetStorageStatus(intptr_t Device);
    int TelRecAPI_GetNetStatus(intptr_t Device);
    int TelRecAPI_GetChannelVoltage(intptr_t Device, int Channel, int *Voltage);
    int TelRecAPI_GetCurrentRecordInfo(intptr_t Device, int Channel, CurrentRecordInfo *Info);
    int TelRecAPI_GetTime(intptr_t Device);
    int TelRecAPI_SetTime(intptr_t Device, TelRec_DateTime *NewDateTime);
    int TelRecAPI_GetPlayBackFileList(intptr_t Device);
    int TelRecAPI_GetBaseSetting(intptr_t Device);
    int TelRecAPI_SetBaseSetting(intptr_t Device, TelRec_BaseSetting *Setting);
    int TelRecAPI_GetChannelSetting(intptr_t Device, int Channel);
    int TelRecAPI_SetChannelSetting(intptr_t Device, int Channel, TelRec_ChannelSetting *Setting);
    int TelRecAPI_GetKeyControlSetting(intptr_t Device);
    int TelRecAPI_SetKeyControlSetting(intptr_t Device, TelRec_KeyControlSetting *Setting);
    int TelRecAPI_GetNetSetting(intptr_t Device);
    int TelRecAPI_SetNetSetting(intptr_t Device, TelRec_NetSetting *Setting);
    int TelRecAPI_GetSMDRSetting(intptr_t Device);
    int TelRecAPI_SetSMDRSetting(intptr_t Device, TelRec_SMDRSetting *Setting);
    int TelRecAPI_GetUserList(intptr_t Device);
    int TelRecAPI_AddUser(intptr_t Device, TelRec_UserInfo *User, const char *Password);
    int TelRecAPI_EditUser(intptr_t Device, TelRec_UserInfo *User, const char *Password);
    int TelRecAPI_DeleteUser(intptr_t Device, TelRec_UserInfo *User);
    int TelRecAPI_RemoveFile(intptr_t Device, const char *FilePath);
    int TelRecAPI_GetLatestRecordTime(intptr_t Device, unsigned char *Year, unsigned char *Month, unsigned char *Day);
    int TelRecAPI_GetEarliestRecordTime(intptr_t Device, unsigned char *Year, unsigned char *Month, unsigned char *Day);
    int TelRecAPI_GetDayListFromMonthDir(intptr_t Device, unsigned char Year, unsigned char Month, unsigned char DayArray[32]);
    int TelRecAPI_EditRecordNotes(intptr_t Device, unsigned short ItemOffset, unsigned char Year, unsigned char Month, unsigned char Day, unsigned char Channel, const char *Notes);
    int TelRecAPI_DeleteRecord(intptr_t Device, RecordDeleteItem *Item);
    int TelRecAPI_StartMonitor(intptr_t Device, unsigned char Channel);
    int TelRecAPI_StopMonitor();
    int TelRecAPI_Dial(intptr_t Device, unsigned char Channel, const char *PhoneNum, int PhoneNumLength);
    int TelRecAPI_GetSMDR(intptr_t Device, int *Length, char **Data);
    int TelRecAPI_Reboot(intptr_t Device);
    int TelRecAPI_GetRecordTimeSetting(intptr_t Device);
    int TelRecAPI_SetRecordTimeSetting(intptr_t Device, TelRec_RecordTimeSetting *Setting);
    int TelRecAPI_GetScheduledRestartSetting(intptr_t Device);
    int TelRecAPI_SetScheduledRestartSetting(intptr_t Device, TelRec_ScheduledRestartSetting *Setting);
    int TelRecAPI_PlayAudioFile(intptr_t Device, int Channel, const char *FileName);
    int TelRecAPI_OffHook(intptr_t Device, int Channel);

    /*Player*/
    int TelRecAPI_PlayerSetVolume(int Volume);
    int TelRecAPI_PlayerWriteData(unsigned char *AudioData);

#ifdef __cplusplus
    int TelRecAPI_SearchDevice(EventCallBack CallBack);
    int TelRecAPI_CreateHeartbeatThread(intptr_t Device, EventCallBack CallBack);
    int TelRecAPI_UploadFile(intptr_t Device, const char *SrcFilePath, const char *UploadDir, const char *UploadFileName, EventCallBack CallBack);
    int TelRecAPI_DownloadFile(intptr_t Device, const char *FilePath, EventCallBack CallBack);

    int TelRecAPI_GetNewVersionInfo(intptr_t Device, EventCallBack CallBack);
    int TelRecAPI_DownloadFirmware(intptr_t Device, const char *FileName, EventCallBack CallBack);
    int TelRecAPI_CheckFirmware(intptr_t Device, const char *FirmwarePath);
    int TelRecAPI_GetClientVersionInfo(EventCallBack CallBack);
#endif

    int TelRecAPI_C_SearchDevice(EventCallBack_C CallBack);
    int TelRecAPI_C_CreateHeartbeatThread(intptr_t Device, EventCallBack_C CallBack);
    int TelRecAPI_C_UploadFile(intptr_t Device, const char *SrcFilePath, const char *UploadDir, const char *UploadFileName, EventCallBack_C CallBack);
    int TelRecAPI_C_DownloadFile(intptr_t Device, const char *FilePath, EventCallBack_C CallBack);

#ifdef __cplusplus
}
#endif

#endif
