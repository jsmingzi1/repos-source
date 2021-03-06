// ConsoleApplication1.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <Windows.h>
#include <Wtsapi32.h>
#include <Tchar.h>

void PrintSessionInfo()
{
	WTS_SESSION_INFOA *pSession = NULL;
	DWORD dwCount = 0;
	BOOL bRet = WTSEnumerateSessionsA(WTS_CURRENT_SERVER_HANDLE, 0, 1, &pSession, &dwCount);
	if (!bRet)
	{
		printf("WTSEnumerateSessions failed with get last error %d.\n", GetLastError());
		return;
	}

	for (int i = 0; i < (int)dwCount; i++)
	{
			printf("WinStationName %s\n", pSession[i].pWinStationName);
			printf("SessionId %d\n", pSession[i].SessionId);
			printf("State %d\n\n\n", pSession[i].State);
			LPSTR pBuffer = NULL;
			DWORD dwBytes = 0;
			WTSQuerySessionInformationA(WTS_CURRENT_SERVER_HANDLE, pSession[i].SessionId, WTSSessionInfoEx, &pBuffer, &dwBytes);
			WTSINFOEX * pInfoEx = (WTSINFOEX*)pBuffer;
			printf("Session Flag is: %d\n\n", pInfoEx->Data.WTSInfoExLevel1.SessionFlags);
			WTSFreeMemory(pBuffer);
	}
	WTSFreeMemory(pSession);
}

void PrintDeskInfo()
{
	HDESK hDesk = OpenInputDesktop(0, FALSE, GENERIC_READ);
	if (hDesk == NULL)
	{
		printf("OpenInputDesktop failed.\n");
		return;
	}
	printf("OpenInputDesktop succeed.\n");
	CloseDesktop(hDesk);
}



void PrintInfo()
{
	wchar_t servername[] = { L"WIN-S9UIFCRBN0T" };
	//HANDLE hServer = WTSOpenServer(servername);
	//if (hServer == NULL)
	//{
	//	printf("WTSOpenServer failed.\n");
	//	return;
	//}
	//printf("WTSOpenServer succeed.\n");
	//PrintSessionInfo();
	//system("pause");
	//WTSCloseServer(hServer);
	HANDLE		    hUserToken = INVALID_HANDLE_VALUE;
	PROCESS_INFORMATION pi;
	STARTUPINFOA	    si;
	BOOL		    bResult = FALSE;
	DWORD		    dwCreationFlags = CREATE_NEW_CONSOLE;
	LPVOID		    pEnv = NULL;

	ZeroMemory(&pi, sizeof(pi));

	ZeroMemory(&si, sizeof(STARTUPINFO));
	si.cb = sizeof(STARTUPINFO);
	char desk[] = { "winsta0\\default" };
	si.lpDesktop = desk;

	char cmd[] = {"notepad.exe"};

	if (!LogonUserA("lcm", NULL, "880330", LOGON32_LOGON_INTERACTIVE, LOGON32_PROVIDER_DEFAULT, &hUserToken))
	{
		printf("LogonUserA failed\n");
		return;
	}
	printf("LogonUserA succeed\n");

	if (!CreateProcessAsUserA(hUserToken, NULL, cmd, NULL, NULL, FALSE, dwCreationFlags, pEnv, NULL, &si, &pi))
	{
		printf("CreateProcessAsUserA failed %d\n", GetLastError());
		CloseHandle(hUserToken);
		return;
	}
	printf("CreateProcessAsUserA succeed\n");
	CloseHandle(hUserToken);

}

int main()
{
	//Sleep(10000);
	PrintSessionInfo();
	//PrintDeskInfo();
	//PrintInfo();
	//PrintSessionInfo();
	system("pause");
    return 0;
}

