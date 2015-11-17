#import "CTimeoutException.h"


// const
static NSString* const CTimeoutExceptionName = @"DGTimeoutException";


// class
@implementation CTimeoutException


// constructors
- (instancetype) init {
    
    // super
    self = [super initWithName:CTimeoutExceptionName reason:nil userInfo:nil];
    if(self == nil) return self;
    
    // return
    return self;
}


@end
