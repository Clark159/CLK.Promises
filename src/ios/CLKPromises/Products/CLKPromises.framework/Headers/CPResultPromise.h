#import <Foundation/Foundation.h>
#import "CPPromiseBase.h"
#import "CPPromise.h"

// class
@class CPPromise;


// class
@interface CPResultPromise : CPPromiseBase


// constructors
- (instancetype) init;


// resolve
- (void) resolve:(id)result;


// then
- (CPPromise*) then:(void(^)(id))onResolved;

- (CPPromise*) then:(void(^)(id))onResolved onRejected:(void(^)(NSException*))onRejected;

- (CPPromise*) then:(void(^)(id))onResolved onRejected:(void(^)(NSException*))onRejected onNotified:(void(^)(CPProgress*))onNotified;

- (CPPromise*) thenPromise:(CPPromise*(^)(id))onResolved;


// thenNew
- (CPResultPromise*) thenNew:(id(^)(id))onResolved;

- (CPResultPromise*) thenNewPromise:(CPResultPromise*(^)(id))onResolved;


// catch
- (CPPromise*) fail:(void(^)(NSException*))onRejected;

- (CPPromise*) failPromise:(CPPromise*(^)(NSException*))onRejected;


// catchNew
- (CPResultPromise*) failNew:(id(^)(NSException*))onRejected;

- (CPResultPromise*) failNewPromise:(CPResultPromise*(^)(NSException*))onRejected;


// progress
- (CPPromise*) progress:(void(^)(CPProgress*))onNotified;

@end
