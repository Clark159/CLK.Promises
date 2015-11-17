#import "CInternalInconsistencyException.h"


@implementation CInternalInconsistencyException


// constructors
- (instancetype) init {
    
    // super
    self = [super initWithName:NSInternalInconsistencyException reason:nil userInfo:nil];
    if(self == nil) return self;
    
    // return
    return self;
}


@end
