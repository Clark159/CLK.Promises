#import "CStringHelper.h"


@implementation CStringHelper


// methods
+ (bool) isNullOrEmpty:(NSString*)value {
    
    // require
    if(value == nil) return true;
    if([value length] == 0) return true;
    
    // return
    return false;
}


@end
