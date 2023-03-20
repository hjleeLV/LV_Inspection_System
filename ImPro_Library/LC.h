/**************************************************************************
*
* filename: LC.h
*
* briefs:  API library interface declaration and the error code
*
**************************************************************************/

#ifndef _LC_H_
#define _LC_H_

#ifdef  _MSC_VER
#pragma comment(linker, "/defaultlib:setupapi.lib")
#endif


#ifdef __cplusplus
extern "C" {
#endif

#ifndef LCAPI
#if defined WIN32 || defined _WIN32
#define LCAPI __stdcall
#else
#define LCAPI
#endif
#endif



// Error Code
#define LC_SUCCESS                            0  // Successful
#define LC_OPEN_DEVICE_FAILED                 1  // Open device failed
#define LC_FIND_DEVICE_FAILED                 2  // No matching device was found
#define LC_INVALID_PARAMETER                  3  // Parameter Error
#define LC_INVALID_BLOCK_NUMBER               4  // Block Error
#define LC_HARDWARE_COMMUNICATE_ERROR         5  // Communication error with hardware
#define LC_INVALID_PASSWORD                   6  // Invalid Password
#define LC_ACCESS_DENIED                      7  // No privileges
#define LC_ALREADY_OPENED                     8  // Device is open
#define LC_ALLOCATE_MEMORY_FAILED             9  // Allocate memory failed
#define LC_INVALID_UPDATE_PACKAGE             10 // Invalid update package
#define LC_SYN_ERROR                          11 // thread Synchronization error
#define LC_OTHER_ERROR                        12 // Other unknown exceptions

// Hardware information structure
typedef struct {
    int    developerNumber; // Developer ID
    char   serialNumber[8]; // Unique Device Serial Number
    int    setDate;         // Manufacturing date
	int    reservation;     // Reserve
}LC_hardware_info;

// Software information structure
typedef struct {
    int    version;        // Software edition
	int    reservation;    // Reserve
} LC_software_info;


// @{
/**
    @LC API function interface
*/

/**
    Open matching device according to Developer ID and Index

    @parameter vendor           [in]  Developer ID (0=All)
    @parameter index            [in]  Device Index (0=1st, and so on)
    @parameter handle           [out] Device handle returned

    @return value
    Successful, 0 returned; failed, predefined error code returned 
*/
int LCAPI LC_open(int vendor, int index, int *handle);

/**
    Close an open device

    @parameter handle           [in]  Device handle opened

    @return value
    Successful, 0 returned; failed, predefined error code returned 
*/
int LCAPI LC_close(int handle);

/**
    Verify device password

    @parameter handle           [in]  Device handle opened
    @parameter type             [in]  Password Type(Admin 0, Generic 1, Authentication 2)
    @parameter passwd           [in]  Password(8 bytes)

    @return value
    Successful, 0 returned; failed, predefined error code returned 
*/
int LCAPI LC_passwd(int handle, int type, unsigned char *passwd);

/**
    Read data from specified block

    @parameter handle           [in]  Device handle opened
    @parameter block            [in]  Number of block to be read
    @parameter buffer           [out] Read data buffer (greater than 512 bytes)

    @return value
    Successful, 0 returned; failed, predefined error code returned 
*/
int LCAPI LC_read(int handle, int block, unsigned char *buffer);

/**
    Write data to specified block

    @parameter handle           [in]  Device handle opened
    @parameter block            [in]  Number of block to be written
    @parameter buffer           [in]  Write data buffer (greater than 512 bytes)

    @return value
    Successful, 0 returned; failed, predefined error code returned 
*/
int LCAPI LC_write(int handle, int block, unsigned char *buffer);

/**
    Encrypt data by AES algorithm

    @parameter handle           [in]  Device handle opened
    @parameter plaintext        [in]  Plain text to be encrypted (16 bytes)
    @parameter ciphertext       [out] Cipher text after being encrypted (16 bytes)

    @return value
    Successful, 0 returned; failed, predefined error code returned 
*/
int LCAPI LC_encrypt(int handle, unsigned char *plaintext,  unsigned char *ciphertext);

/**
    Decrypt data by AES algorithm

    @parameter handle           [in]  Device handle opened
    @parameter ciphertext       [in]  Cipher text to be decrypted (16 bytes)
    @parameter plaintext        [out] Plain text after being decrypted (16 bytes)

    @return value
    Successful, 0 returned; failed, predefined error code returned 
*/
int LCAPI LC_decrypt(int handle, unsigned char *ciphertext, unsigned char *plaintext);

/**
    Setting new password requires developer privileges.

    @parameter handle           [in]  Device handle opened
    @parameter type             [in]  Password Type(Admin 0, Generic 1, Authentication 2)
    @parameter newpasswd        [in]  Password(8 bytes)
    @parameter retries          [in]  Error Count (1~15), -1 disables error count

    @return value
    Successful, 0 returned; failed, predefined error code returned 
*/
int LCAPI LC_set_passwd(int handle, int type, unsigned char *newpasswd, int retries);

/**
    Change password

    @parameter handle           [in]  Device handle opened
    @parameter type             [in]  Password type (Authentication 2)
    @parameter oldpasswd        [in]  Old Password (8 bytes)
    @parameter newpasswd        [in]  New Password (8 bytes)

    @return value
    Successful, 0 returned; failed, predefined error code returned 
*/
int LCAPI LC_change_passwd(int handle, int type, unsigned char *oldpasswd, unsigned char *newpasswd);

/**
    Retrieve hardware information 

    @parameter handle           [in]  Device handle opened
    @parameter info             [out] Hardware information

    @return value
    Successful, 0 returned; failed, predefined error code returned 
*/
int LCAPI LC_get_hardware_info(int handle, LC_hardware_info *info);

/**
    Retrieve software information

    @parameter info             [out] Software information

    @return value
    Successful, 0 returned; failed, predefined error code returned 
*/
int LCAPI LC_get_software_info(LC_software_info *info);

/**
    Calculate hmac value by hardware

    @parameter handle           [in]  Device handle opened
    @parameter text             [in]  Data to be used in calculating hmac value
    @parameter textlen          [in]  Data length (>=0)
    @parameter digest           [out] Hmac value (20 bytes)

    @return value
    Successful, 0 returned; failed, predefined error code returned 
*/
//int LCAPI LC_hmac(int handle, unsigned char *text, int textlen, unsigned char *digest);

/**
    Update block remotely

    @parameter handle           [in]  Device handle opened
    @parameter buffer           [in]  Update the buffer area of data package 


    @return value
    Successful, 0 returned; failed, predefined error code returned 
*/
int LCAPI LC_update(int handle, unsigned char *buffer);

/**
    Set  key

    @parameter handle           [in]  Device handle opened
    @parameter type             [in]  Key type (0-Remote update key, 1-Authentication key)
	@parameter key              [in]  key (20 bytes)


    @return value
    Successful, 0 returned; failed, predefined error code returned 
*/
int LCAPI LC_set_key(int handle, int type, unsigned char *key);

/**
    Generate update package
	@parameter serial           [in]  Serial Number of dongle to be updated
	@parameter block            [in]  Number of block to be updated
	@parameter buffer           [in]  Update content (384 bytes for Block 3, 512 bytes for Block 0~2)
	@parameter key              [in]  Remote update key(20 bytes)
	@parameter uptPkg           [out] Update Package(549 bytes)

	@return value
    Successful, 0 returned; failed, predefined error code returned 
*/
int LCAPI LC_gen_update_pkg(unsigned char * serial, int block, unsigned char *buffer, unsigned char *key, unsigned char *uptPkg);

#ifdef __cplusplus
}
#endif

#endif /* end of "#ifndef _LC_H_" */
