#import <Security/Security.h>
#import <Foundation/Foundation.h>

static CFDictionaryRef Query(NSString* key)
{
    return (__bridge CFDictionaryRef)@{
        (__bridge id)kSecClass       : (__bridge id)kSecClassGenericPassword,
        (__bridge id)kSecAttrService : @"LiteDBService",
        (__bridge id)kSecAttrAccount : key
    };
}

extern "C"
{
bool _sp_keychain_save(const char* c_key, const char* c_val)
{
    NSString *key = [NSString stringWithUTF8String:c_key];
    NSData   *val = [[NSString stringWithUTF8String:c_val] dataUsingEncoding:NSUTF8StringEncoding];

    SecItemDelete(Query(key)); // 同じキーがあれば削除
    NSDictionary* add = @{
        (__bridge id)kSecClass       : (__bridge id)kSecClassGenericPassword,
        (__bridge id)kSecAttrService : @"LiteDBService",
        (__bridge id)kSecAttrAccount : key,
        (__bridge id)kSecValueData   : val
    };
    return (SecItemAdd((__bridge CFDictionaryRef)add, nil) == errSecSuccess);
}

const char* _sp_keychain_load(const char* c_key)
{
    NSString* key = [NSString stringWithUTF8String:c_key];
    CFTypeRef data = nil;
    NSMutableDictionary* q = [(__bridge NSDictionary*)Query(key) mutableCopy];
    q[(__bridge id)kSecReturnData] = @YES;
    q[(__bridge id)kSecMatchLimit] = (__bridge id)kSecMatchLimitOne;

    if (SecItemCopyMatching((__bridge CFDictionaryRef)q, &data) == errSecSuccess)
    {
        NSString* str = [[NSString alloc] initWithData:(__bridge NSData*)data
                                              encoding:NSUTF8StringEncoding];
        return strdup([str UTF8String]); // 呼び出し側で解放しないが OK（短命）
    }
    return nullptr;
}

void _sp_keychain_delete(const char* c_key)
{
    NSString* key = [NSString stringWithUTF8String:c_key];
    SecItemDelete(Query(key));
}
}
