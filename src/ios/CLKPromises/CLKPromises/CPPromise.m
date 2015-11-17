#import <CLK/CLK.h>
#import "CPPromise.h"


@implementation CPPromise


// fields
{
    
@private id(^_passResolved)(void);
    
@private id(^_passRejected)(NSException*);
    
@private void(^_passNotified)(CPProgress*);
    
}


// constructors
- (instancetype) init {
    
    // super
    self = [super init];
    if(self == nil) return self;
    
    // default
    _passResolved = nil;
    _passRejected = nil;
    _passNotified = nil;
    
    // return
    return self;
}


// methods
- (CPPromise*) pushThen:(id(^)(void))onResolved onResolvedResultType:(enum ResultType)onResolvedResultType
             onRejected:(id(^)(NSException*))onRejected onRejectedResultType:(enum ResultType)onRejectedResultType
             onNotified:(void(^)(CPProgress*))onNotified
{
    // contracts
    if (onResolved == nil) @throw [[CInvalidArgumentException alloc] init];
    if (onRejected == nil) @throw [[CInvalidArgumentException alloc] init];
    if (onNotified == nil) @throw [[CInvalidArgumentException alloc] init];
    
    // promise
    CPPromise* thenPromise = [[CPPromise alloc] init];
    
    // resolveHandler
    void(^resolveHandler)(id) = [^void(id result)
                                 {
                                     @try
                                     {
                                         // execute
                                         id resultObject = onResolved();
                                         
                                         // distribute
                                         switch (onResolvedResultType)
                                         {
                                             case Empty:
                                                 [thenPromise resolve];
                                                 break;
                                                 
                                             case EmptyPromise:
                                                 if (resultObject != nil) {
                                                     [(CPPromise*)resultObject
                                                      then:[^void(void){ [thenPromise resolve]; } copy]
                                                      onRejected:[^void(NSException* thenError){ [thenPromise reject:thenError]; } copy]
                                                      onNotified:[^void(CPProgress* thenProgress){ [thenPromise notify:thenProgress]; } copy]
                                                      ];
                                                 }
                                                 else { @throw [[NSException alloc] initWithName:@"Invalid Result" reason:nil userInfo:nil]; }
                                                 break;
                                                 
                                             default:
                                                 @throw [[NSException alloc] initWithName:@"Invalid Result Type" reason:nil userInfo:nil];
                                         }
                                     }
                                     @catch (NSException* ex)
                                     {
                                         [thenPromise reject:ex];
                                     }
                                 } copy];
    
    // rejectHandler
    void(^rejectHandler)(NSException*) = [^void(NSException* error)
                                          {
                                              @try
                                              {
                                                  // execute
                                                  id resultObject = onRejected(error);
                                                  
                                                  // distribute
                                                  switch (onRejectedResultType)
                                                  {
                                                      case Empty:
                                                          [thenPromise resolve];
                                                          break;
                                                          
                                                      case EmptyPromise:
                                                          if (resultObject != nil) {
                                                              [(CPPromise*)resultObject
                                                               then:[^void(void){ [thenPromise resolve]; } copy]
                                                               onRejected:[^void(NSException* thenError){ [thenPromise reject:thenError]; } copy]
                                                               onNotified:[^void(CPProgress* thenProgress){ [thenPromise notify:thenProgress]; } copy]
                                                               ];
                                                          }
                                                          else { @throw [[NSException alloc] initWithName:@"Invalid Result" reason:nil userInfo:nil]; }
                                                          break;
                                                          
                                                      default:
                                                          @throw [[NSException alloc] initWithName:@"Invalid Result Type" reason:nil userInfo:nil];
                                                  }
                                              }
                                              @catch (NSException* ex)
                                              {
                                                  [thenPromise reject:ex];
                                              }
                                          } copy];
    
    // notifyHandler
    void(^notifyHandler)(CPProgress*) = [^void(CPProgress* progress)
                                         {
                                             @try
                                             {
                                                 // execute
                                                 onNotified(progress);
                                                 
                                                 // distribute
                                                 [thenPromise notify:progress];
                                             }
                                             @catch (NSException* ex)
                                             {
                                                 [thenPromise reject:ex];
                                             }
                                         } copy];
    
    // push
    [self push:resolveHandler rejectHandler:rejectHandler notifyHandler:notifyHandler];
    
    // return
    return thenPromise;
}

- (CPResultPromise*) pushThenNew:(id(^)(void))onResolved onResolvedResultType:(enum ResultType)onResolvedResultType
                      onRejected:(id(^)(NSException*))onRejected onRejectedResultType:(enum ResultType)onRejectedResultType
                      onNotified:(void(^)(CPProgress*))onNotified
{
    // contracts
    if (onResolved == nil) @throw [[CInvalidArgumentException alloc] init];
    if (onRejected == nil) @throw [[CInvalidArgumentException alloc] init];
    if (onNotified == nil) @throw [[CInvalidArgumentException alloc] init];
    
    // promise
    CPResultPromise* thenPromise = [[CPResultPromise alloc] init];
    
    // resolveHandler
    void(^resolveHandler)(id) = [^void(id result)
                                 {
                                     @try
                                     {
                                         // execute
                                         id resultObject = onResolved();
                                         
                                         // distribute
                                         switch (onResolvedResultType)
                                         {
                                             case Empty:
                                                 [thenPromise resolve:nil];
                                                 break;
                                                 
                                             case EmptyPromise:
                                                 if (resultObject != nil) {
                                                     [(CPPromise*)resultObject
                                                      then:[^void(void){ [thenPromise resolve:nil]; } copy]
                                                      onRejected:[^void(NSException* thenError){ [thenPromise reject:thenError]; } copy]
                                                      onNotified:[^void(CPProgress* thenProgress){ [thenPromise notify:thenProgress]; } copy]
                                                      ];
                                                 }
                                                 else { @throw [[NSException alloc] initWithName:@"Invalid Result" reason:nil userInfo:nil]; }
                                                 break;
                                                 
                                             case New:
                                                 [thenPromise resolve:resultObject];
                                                 break;
                                                 
                                             case NewPromise:
                                                 if (resultObject != nil) {
                                                     [(CPResultPromise*)resultObject
                                                      then:[^void(id thenResult){ [thenPromise resolve:thenResult]; } copy]
                                                      onRejected:[^void(NSException* thenError){ [thenPromise reject:thenError]; } copy]
                                                      onNotified:[^void(CPProgress* thenProgress){ [thenPromise notify:thenProgress]; } copy]
                                                      ];
                                                 }
                                                 else { @throw [[NSException alloc] initWithName:@"Invalid Result" reason:nil userInfo:nil]; }
                                                 break;
                                                 
                                             default:
                                                 @throw [[NSException alloc] initWithName:@"Invalid Result Type" reason:nil userInfo:nil];
                                         }
                                     }
                                     @catch (NSException* ex)
                                     {
                                         [thenPromise reject:ex];
                                     }
                                 } copy];
    
    // rejectHandler
    void(^rejectHandler)(NSException*) = [^void(NSException* error)
                                          {
                                              @try
                                              {
                                                  // execute
                                                  id resultObject = onRejected(error);
                                                  
                                                  // distribute
                                                  switch (onRejectedResultType)
                                                  {
                                                      case Empty:
                                                          [thenPromise resolve:nil];
                                                          break;
                                                          
                                                      case EmptyPromise:
                                                          if (resultObject != nil) {
                                                              [(CPPromise*)resultObject
                                                               then:[^void(void){ [thenPromise resolve:nil]; } copy]
                                                               onRejected:[^void(NSException* thenError){ [thenPromise reject:thenError]; } copy]
                                                               onNotified:[^void(CPProgress* thenProgress){ [thenPromise notify:thenProgress]; } copy]
                                                               ];
                                                          }
                                                          else { @throw [[NSException alloc] initWithName:@"Invalid Result" reason:nil userInfo:nil]; }
                                                          break;
                                                          
                                                      case New:
                                                          [thenPromise resolve:resultObject];
                                                          break;
                                                          
                                                      case NewPromise:
                                                          if (resultObject != nil) {
                                                              [(CPResultPromise*)resultObject
                                                               then:[^void(id thenResult){ [thenPromise resolve:thenResult]; } copy]
                                                               onRejected:[^void(NSException* thenError){ [thenPromise reject:thenError]; } copy]
                                                               onNotified:[^void(CPProgress* thenProgress){ [thenPromise notify:thenProgress]; } copy]
                                                               ];
                                                          }
                                                          else { @throw [[NSException alloc] initWithName:@"Invalid Result" reason:nil userInfo:nil]; }
                                                          break;
                                                          
                                                      default:
                                                          @throw [[NSException alloc] initWithName:@"Invalid Result Type" reason:nil userInfo:nil];
                                                  }
                                              }
                                              @catch (NSException* ex)
                                              {
                                                  [thenPromise reject:ex];
                                              }
                                          } copy];
    
    // notifyHandler
    void(^notifyHandler)(CPProgress*) = [^void(CPProgress* progress)
                                         {
                                             @try
                                             {
                                                 // execute
                                                 onNotified(progress);
                                                 
                                                 // distribute
                                                 [thenPromise notify:progress];
                                             }
                                             @catch (NSException* ex)
                                             {
                                                 [thenPromise reject:ex];
                                             }
                                         } copy];
    
    // push
    [self push:resolveHandler rejectHandler:rejectHandler notifyHandler:notifyHandler];
    
    // return
    return thenPromise;
}


- (id(^)(void)) passResolved
{
    if (_passResolved == nil)
    {
        _passResolved = [^id(void)
                         {
                             return nil;
                         } copy];
    }
    return _passResolved;
}

- (id(^)(NSException*)) passRejected
{
    if (_passRejected == nil)
    {
        _passRejected = [^id(NSException* error)
                         {
                             @throw error;
                         } copy];
    }
    return _passRejected;
}

- (void(^)(CPProgress*)) passNotified
{
    if (_passNotified == nil)
    {
        _passNotified = [^void(CPProgress* progress)
                         {
                             
                         } copy];
    }
    return _passNotified;
}


// resolve
- (void) resolve
{
    // resolve
    [self resolveBase:nil];
}


// then
- (CPPromise*) then:(void(^)(void))onResolved
{
    return [self
            pushThen:[^id(void){ onResolved(); return nil; } copy] onResolvedResultType:Empty
            onRejected:[self passRejected] onRejectedResultType:Empty
            onNotified:[self passNotified]
            ];
}

- (CPPromise*) then:(void(^)(void))onResolved onRejected:(void(^)(NSException*))onRejected
{
    return [self
            pushThen:[^id(void){ onResolved(); return nil; } copy] onResolvedResultType:Empty
            onRejected:[^id(NSException* error){ onRejected(error); return nil; } copy] onRejectedResultType:Empty
            onNotified:[self passNotified]
            ];
}

- (CPPromise*) then:(void(^)(void))onResolved onRejected:(void(^)(NSException*))onRejected onNotified:(void(^)(CPProgress*))onNotified
{
    return [self
            pushThen:[^id(void){ onResolved(); return nil; } copy] onResolvedResultType:Empty
            onRejected:[^id(NSException* error){ onRejected(error); return nil; } copy] onRejectedResultType:Empty
            onNotified:onNotified
            ];
}

- (CPPromise*) thenPromise:(CPPromise*(^)(void))onResolved
{
    return [self
            pushThen:[^id(void){ return onResolved(); } copy] onResolvedResultType:EmptyPromise
            onRejected:[self passRejected] onRejectedResultType:Empty
            onNotified:[self passNotified]
            ];
}


// thenNew
- (CPResultPromise*) thenNew:(id(^)(void))onResolved
{
    return [self
            pushThenNew:[^id(void){ return onResolved(); } copy] onResolvedResultType:New
            onRejected:[self passRejected] onRejectedResultType:Empty
            onNotified:[self passNotified]
            ];
}

- (CPResultPromise*) thenNewPromise:(CPResultPromise*(^)(void))onResolved
{
    return [self
            pushThenNew:[^id(void){ return onResolved(); } copy] onResolvedResultType:NewPromise
            onRejected:[self passRejected] onRejectedResultType:Empty
            onNotified:[self passNotified]
            ];
}


// catch
- (CPPromise*) fail:(void(^)(NSException*))onRejected
{
    return [self
            pushThen:[self passResolved] onResolvedResultType:Empty
            onRejected:[^id(NSException* error){ onRejected(error); return nil; } copy] onRejectedResultType:Empty
            onNotified:[self passNotified]
            ];
}

- (CPPromise*) failPromise:(CPPromise*(^)(NSException*))onRejected
{
    return [self
            pushThen:[self passResolved] onResolvedResultType:Empty
            onRejected:[^id(NSException* error){ return onRejected(error); } copy] onRejectedResultType:EmptyPromise
            onNotified:[self passNotified]
            ];
}


// catchNew
- (CPResultPromise*) failNew:(id(^)(NSException*))onRejected
{
    return [self
            pushThenNew:[self passResolved] onResolvedResultType:Empty
            onRejected:[^id(NSException* error){ onRejected(error); return nil; } copy] onRejectedResultType:New
            onNotified:[self passNotified]
            ];
}

- (CPResultPromise*) failNewPromise:(CPResultPromise*(^)(NSException*))onRejected
{
    return [self
            pushThenNew:[self passResolved] onResolvedResultType:Empty
            onRejected:[^id(NSException* error){ return onRejected(error); } copy] onRejectedResultType:NewPromise
            onNotified:[self passNotified]
            ];
}


// progress
- (CPPromise*) progress:(void(^)(CPProgress*))onNotified
{
    return [self
            pushThen:[self passResolved] onResolvedResultType:Empty
            onRejected:[self passRejected] onRejectedResultType:Empty
            onNotified:onNotified
            ];
}



@end
