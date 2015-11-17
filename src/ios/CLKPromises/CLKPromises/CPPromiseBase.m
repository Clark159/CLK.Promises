#import <CLK/CLK.h>
#import "CPPromiseBase.h"


// class
@implementation CPPromiseBase


// fields
{
    
@private NSObject* _syncRoot;
    
@private enum PromiseState _state ;
    
@private id _result ;
    
@private NSException* _error;
    
    
@private NSMutableArray* _resolveHandlers;
    
@private NSMutableArray* _rejectHandlers;
    
@private NSMutableArray* _notifyHandlers;
    
@private NSMutableArray* _notifyHandlersSnapshot;
    
}


// constructors
- (instancetype) init {
    
    // super
    self = [super init];
    if(self == nil) return self;
    
    // default
    _syncRoot = [[NSObject alloc] init];
    _state = Pending;
    _result = nil;
    _error = nil;
    _resolveHandlers = nil;
    _rejectHandlers = nil;
    _notifyHandlers = nil;
    _notifyHandlersSnapshot = nil;
    
    // return
    return self;
}


// methods
- (void) push:(void(^)(id))resolveHandler rejectHandler:(void(^)(NSException*))rejectHandler notifyHandler:(void(^)(CPProgress*))notifyHandler {
    
    // contracts
    if (resolveHandler == nil) @throw [[CInvalidArgumentException alloc] init];
    if (rejectHandler == nil) @throw [[CInvalidArgumentException alloc] init];
    if (notifyHandler == nil) @throw [[CInvalidArgumentException alloc] init];
    
    // sync
    @synchronized (_syncRoot)
    {
        // resolveHandler
        if (_state == Pending)
        {
            if(_resolveHandlers == nil)
            {
                _resolveHandlers = [[NSMutableArray alloc] init];
            }
            [_resolveHandlers addObject:resolveHandler];
        }
        
        // rejectHandler
        if (_state == Pending)
        {
            if (_rejectHandlers == nil)
            {
                _rejectHandlers = [[NSMutableArray alloc] init];
            }
            [_rejectHandlers addObject:rejectHandler];
        }
        
        // notifyHandler
        if (_notifyHandlers == nil)
        {
            _notifyHandlers = [[NSMutableArray alloc] init];
        }
        [_notifyHandlers addObject:notifyHandler];
        _notifyHandlersSnapshot = nil;
        
        // pending
        if (_state == Pending)
        {
            return;
        }
    }
    
    // resolved
    if (_state == Resolved)
    {
        resolveHandler(_result);
    }
    
    // rejected
    if (_state == Rejected)
    {
        rejectHandler(_error);
    }
}

- (void) resolveBase:(id)result {
    
    // sync
    @synchronized (_syncRoot)
    {
        // state
        if (_state != Pending) return;
        _state = Resolved;
        
        // results
        _result = result;
        _error = nil;
    }
    
    // handlers
    NSMutableArray* resolveHandlers = _resolveHandlers;
    if (resolveHandlers == nil) return;
    
    // resolve
    for (void(^resolveHandler)(id) in resolveHandlers)
    {
        resolveHandler(result);
    }
}

- (void) reject:(NSException*)error {
    
    // contracts
    if (error == nil) @throw [[CInvalidArgumentException alloc] init];

    // sync
    @synchronized (_syncRoot)
    {
        // state
        if (_state != Pending) return;
        _state = Rejected;
        
        // result
        _result = nil;
        _error = error;
    }
    
    // handlers
    NSMutableArray* rejectHandlers = _rejectHandlers;
    if (rejectHandlers == nil) return;
    
    // reject
    for (void(^rejectHandler)(NSException*) in rejectHandlers)
    {
        rejectHandler(error);
    }
}

- (void) notify:(CPProgress*)progress {

    // contracts
    if (progress == nil) @throw [[CInvalidArgumentException alloc] init];
        
    // handlers
    NSMutableArray* notifyHandlers = nil;
    @synchronized (_syncRoot)
    {
        if (_notifyHandlersSnapshot == nil && _notifyHandlers != nil)
        {
            _notifyHandlersSnapshot = [[NSMutableArray alloc] init];
            for (void(^notifyHandler)(CPProgress*) in _notifyHandlers)
            {
                [_notifyHandlersSnapshot addObject:notifyHandler];
            }
        }
        notifyHandlers = _notifyHandlersSnapshot;
    }
    if (notifyHandlers == nil) return;
    
    // notify
    for (void(^notifyHandler)(CPProgress*) in notifyHandlers)
    {
        notifyHandler(progress);
    }
}


@end
