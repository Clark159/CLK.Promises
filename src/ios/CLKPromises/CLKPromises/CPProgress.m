#import "CPProgress.h"


@implementation CPProgress


// constructors
- (instancetype) initWithLocalizedDescription:(NSString*)localizedDescription completedCount:(int)completedCount totalCount:(int)totalCount {
    
    // super
    self = [super init];
    if(self == nil) return self;
    
    // default
    _localizedDescription = localizedDescription;
    _completedCount = completedCount;
    _totalCount = totalCount;
    
    // return
    return self;
}


@end
