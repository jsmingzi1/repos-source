// ConsoleApplication1_demo.cpp : Defines the entry point for the console application.
//

#include "stdafx.h"
#include <Windows.h>



int main()
{
	const char *str = "880330";
	printf("char: %s\n", str);

	const WCHAR *pwcsName;
	// required size
	int nChars = MultiByteToWideChar(CP_ACP, 0, str, -1, NULL, 0);
	// allocate it
	pwcsName = new WCHAR[nChars];
	MultiByteToWideChar(CP_ACP, 0, str, -1, (LPWSTR)pwcsName, nChars);
	// use it....
	wprintf(L"wchar: %s\n", pwcsName);
	// delete it
	delete[] pwcsName;

	system("pause");
    return 0;
}

