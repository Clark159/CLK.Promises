#import <CLK/CLK.h>
#import <CLKPromises/CLKPromises.h>
#import "ViewController.h"


@implementation ViewController


// methods
- (void)viewDidLoad {
    
    // super
    [super viewDidLoad];
    
    // test
    [self promiseTests];
}


- (void) promiseTests
{
    // this
    __block ViewController* this = self;
    
    // promise
    CPPromise* promise = [[CPPromise alloc]init];
    
    [[[[[[[[[[[[[[[[[
               promise
               
               // ========== then ==========
               then:[^void(void)
                     {
                         [this writeLine:@"AAA"];
                     }copy]]
              
              // thenPromise - resolve
              thenPromise:[^CPPromise*(void)
                           {
                               CPPromise* newPromise = [[CPPromise alloc]init];
                               [newPromise resolve];
                               return newPromise;
                           }copy]]
             
             then:[^void(void)
                   {
                       [this writeLine:@"BBB"];
                   }copy]]
            
            // thenPromise - reject
            thenPromise:[^CPPromise*(void)
                         {
                             CPPromise* newPromise = [[CPPromise alloc]init];
                             [newPromise reject:[[NSException alloc] initWithName:@"CCC" reason:nil userInfo:nil]];
                             return newPromise;
                         }copy]]
            
           fail:[^void(NSException* error)
                 {
                     [this writeLine:error.name];
                 }copy]]
          
          
           // ========== thenNew ==========
          thenNew:[^NSString*(void)
                       {
                           return @"DDD";
                       }copy]]

          then:[^void(NSString* result)
                {
                    [this writeLine:result];
                }copy]]
            
          // thenNewPromise - resolve
          thenNewPromise:[^CPResultPromise*(void)
                       {
                           CPResultPromise* newPromise = [[CPResultPromise alloc]init];
                           [newPromise resolve:@"EEE"];
                           return newPromise;
                       }copy]]
         
         then:[^void(NSString* result)
               {
                   [this writeLine:result];
               }copy]]
          
          
           // thenPromise - reject
           thenNewPromise:[^CPResultPromise*(void)
                        {
                            CPResultPromise* newPromise = [[CPResultPromise alloc]init];
                            [newPromise reject:[[NSException alloc] initWithName:@"FFF" reason:nil userInfo:nil]];
                            return newPromise;
                        }copy]]

         fail:[^void(NSException* error)
               {
                   [this writeLine:error.name];
               }copy]]

        
        
          // ========== throw ==========
          then:[^void(void)
                {
                    @throw [[NSException alloc] initWithName:@"III" reason:nil userInfo:nil];
                }copy]]
         
         fail:[^void(NSException* error)
               {
                   @throw error;
               }copy]]
        
        fail:[^void(NSException* error)
              {
                  [this writeLine:error.name];
              }copy]]
       
       // ========== end ==========
       progress:[^void(CPProgress* progressx)
                 {
                     [this writeLine:[NSString stringWithFormat:@"%@%@", @"Progress:", progressx.localizedDescription]];
                 }copy]]
      
      fail:[^void(NSException* error)
            {
                [this writeLine:[NSString stringWithFormat:@"%@%@", @"Fail:", error.name]];
            }copy]]
     
     then:[^void(void)
           {
               [this writeLine:@"End"];
           }copy]]
    ;
    
    // operate
    [promise notify:[[CPProgress alloc]initWithLocalizedDescription:@"0%" completedCount:0 totalCount:100]];
    [promise notify:[[CPProgress alloc]initWithLocalizedDescription:@"50%" completedCount:50 totalCount:100]];
    [promise notify:[[CPProgress alloc]initWithLocalizedDescription:@"100%" completedCount:100 totalCount:100]];
    [promise resolve];
}

- (void) writeLine:(NSString*)message
{
    // writeLine
    self.textView.text = [self.textView.text stringByAppendingString:message];
    self.textView.text = [self.textView.text stringByAppendingString:@"\n"];
}


@end
