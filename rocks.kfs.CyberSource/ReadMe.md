When using this Gateway provider with the old "legacy" recurring billing that is found under Virtual Terminal > Recurring Billing (not Token Management or the root Recurring billing), you may receive errors such as this:

```Exception on calling the API : Error calling PostCustomer: {"errors":[{"type":"forbidden","message":"Request not permitted"}]}```

Due to lack of modern REST API support, the legacy recurring billing options are not supported. Please contact CyberSource for more information.

See https://developer.cybersource.com/api-reference-assets/index.html#recurring-billing-subscriptions_plans_recurring-billing-subscriptions or https://developer.cybersource.com/api-reference-assets/index.html#token-management for api reference.