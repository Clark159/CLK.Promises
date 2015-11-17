#import "CInvalidArgumentException.h"


@implementation CInvalidArgumentException


// constructors
- (instancetype) init {
    
    // super
    self = [super initWithName:NSInvalidArgumentException reason:nil userInfo:nil];
    if(self == nil) return self;
    
    // return
    return self;
}


@end
