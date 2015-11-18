#import <UIKit/UIKit.h>
#import "CPPromiseBase.h"
#import "CPResultPromise.h"


// class
@class CPResultPromise;


// class
@interface CPPromise : CPPromiseBase


// constructors
- (instancetype) init;


// resolve
- (void) resolve;


// then
- (CPPromise*) then:(void(^)(void))onResolved;

- (CPPromise*) then:(void(^)(void))onResolved onRejected:(void(^)(NSException*))onRejected;

- (CPPromise*) then:(void(^)(void))onResolved onRejected:(void(^)(NSException*))onRejected onNotified:(void(^)(CPProgress*))onNotified;

- (CPPromise*) thenPromise:(CPPromise*(^)(void))onResolved;


// thenNew
- (CPResultPromise*) thenNew:(id(^)(void))onResolved;

- (CPResultPromise*) thenNewPromise:(CPResultPromise*(^)(void))onResolved;


// catch
- (CPPromise*) fail:(void(^)(NSException*))onRejected;

- (CPPromise*) failPromise:(CPPromise*(^)(NSException*))onRejected;


// catchNew
- (CPResultPromise*) failNew:(id(^)(NSException*))onRejected;

- (CPResultPromise*) failNewPromise:(CPResultPromise*(^)(NSException*))onRejected;


// progress
- (CPPromise*) progress:(void(^)(CPProgress*))onNotified;


// all
+ (CPPromise*) allPromise:(NSMutableArray*)promiseList;

+ (CPResultPromise*) allNewPromise:(NSMutableArray*)promiseList;


@end
