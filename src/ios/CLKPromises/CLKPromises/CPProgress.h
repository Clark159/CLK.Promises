#import <Foundation/Foundation.h>


@interface CPProgress : NSObject


// constructors
- (instancetype) initWithLocalizedDescription:(NSString*)localizedDescription completedCount:(int)completedCount totalCount:(int)totalCount;


// properties
@property (readonly, retain) NSString* localizedDescription;

@property (readonly, assign) int completedCount;

@property (readonly, assign) int totalCount;


@end
