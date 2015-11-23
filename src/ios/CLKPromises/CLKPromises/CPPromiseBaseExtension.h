#import "CPPromiseBase.h"


// enumerations
enum PromiseState{
    Pending,  // unresolved
    Resolved, // has-resolution
    Rejected, // has-rejection
};

enum ResultType{
    Empty,
    EmptyPromise,
    New,
    NewPromise,
};


// class
@interface CPPromiseBase ()


// methods
- (void) push:(void(^)(id))resolveHandler rejectHandler:(void(^)(NSException*))rejectHandler notifyHandler:(void(^)(CPProgress*))notifyHandler;

- (void) resolveBase:(id)result;


@end
