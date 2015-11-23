#import <Foundation/Foundation.h>
#import "CPProgress.h"


// class
@interface CPPromiseBase : NSObject


// constructors
- (instancetype) init;


// methods
- (void) reject:(NSException*)error;

- (void) notify:(CPProgress*)progress;


@end
