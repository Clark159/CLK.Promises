#import <Foundation/Foundation.h>


@interface CPProgress : NSObject


// constructors
- (instancetype) initWithLocalizedDescription:(NSString*)localizedDescription completedCount:(NSInteger)completedCount totalCount:(NSInteger)totalCount;


// properties
@property (readonly, retain) NSString* localizedDescription;

@property (readonly, assign) NSInteger completedCount;

@property (readonly, assign) NSInteger totalCount;


@end
