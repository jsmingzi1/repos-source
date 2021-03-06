// ConsoleApplication1.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <Windows.h>

#define BUFSIZE 512

typedef struct Stu {
	int Age;
	char Name[20] = { 0 };
};

DWORD WINAPI InstanceThread(LPVOID);

int main()
{
	BOOL   fConnected = FALSE;
	DWORD  dwThreadId = 0;
	HANDLE hPipe = INVALID_HANDLE_VALUE, hThread = NULL;
	LPCSTR lpszPipename = "\\\\.\\pipe\\TestPipe";

	// The main loop creates an instance of the named pipe and 
	// then waits for a client to connect to it. When the client 
	// connects, a thread is created to handle communications 
	// with that client, and this loop is free to wait for the
	// next client connect request. It is an infinite loop.



		_tprintf(TEXT("\nPipe Server: Main thread awaiting client connection on %s\n"), lpszPipename);
		hPipe = CreateNamedPipeA(
			lpszPipename,             // pipe name 
			PIPE_ACCESS_DUPLEX,       // read/write access 
			PIPE_TYPE_MESSAGE |       // message type pipe 
			PIPE_READMODE_MESSAGE |   // message-read mode 
			PIPE_WAIT,                // blocking mode 
			PIPE_UNLIMITED_INSTANCES, // max. instances  
			BUFSIZE,                  // output buffer size 
			BUFSIZE,                  // input buffer size 
			0,                        // client time-out 
			NULL);                    // default security attribute 

		if (hPipe == INVALID_HANDLE_VALUE)
		{
			_tprintf(TEXT("CreateNamedPipe failed, GLE=%d.\n"), GetLastError());
			return -1;
		}

		// Wait for the client to connect; if it succeeds, 
		// the function returns a nonzero value. If the function
		// returns zero, GetLastError returns ERROR_PIPE_CONNECTED. 

		fConnected = ConnectNamedPipe(hPipe, NULL) ?
			TRUE : (GetLastError() == ERROR_PIPE_CONNECTED);

		if (fConnected)
		{
			printf("Client connected, creating a processing thread.\n");
			byte buff[BUFSIZE] = { 0 };
			DWORD dwLength = 0;
			BOOL fSuccess = ReadFile(
				hPipe,        // handle to pipe 
				buff,    // buffer to receive data 
				BUFSIZE, // size of buffer 
				&dwLength, // number of bytes read 
				NULL);        // not overlapped I/O 

			if (!fSuccess || dwLength == 0)
			{
				if (GetLastError() == ERROR_BROKEN_PIPE)
				{
					_tprintf(TEXT("InstanceThread: client disconnected.\n"), GetLastError());
				}
				else
				{
					_tprintf(TEXT("InstanceThread ReadFile failed, GLE=%d.\n"), GetLastError());
				}
			}
			else
			{
				Stu *stu = (Stu *)buff;
				printf("%d, %s\n", stu->Age, stu->Name);
			}
			
		}
		else
			// The client could not connect, so close the pipe. 
			CloseHandle(hPipe);

		FlushFileBuffers(hPipe);
		DisconnectNamedPipe(hPipe);
		CloseHandle(hPipe);
		system("pause");

	return 0;
}

