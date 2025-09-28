#### **About:**

You are part of a team designing and building a platform for the streaming of video content on demand

(VOD). Think Netflix, Disney+, or Amazon Prime Video - but this is our next-generation streaming service that

needs to deliver personalized content to millions of users worldwide.

Our platform serves content through dynamic shelves - curated rows of shows and movies that adapt to user

preferences, trending content, and editorial decisions. The home page experience is critical: users expect

instant loading of above-the-fold content while personalized recommendations load seamlessly below.

Your mission is to build the Shelves Service - the backbone that orchestrates content delivery from multiple

microservices into the fast-loading applications experience our users love.



#### **Notes:**

The home page experience is critical:

Users expect instant loading of above-the-fold content while personalized recommendations load seamlessly below.

However, Data Source Type handling should be abstracted from the front-end platforms, which should only be aware of how to render Shelf types.

In other words, the data source types should be resolved by the back end.



#### **Questions:**

If the layout is only known by ShelvesService once it makes a call to LayoutService,

how can the front-end know to quickly load just the nonpersonalised (which is based on data source type)?



#### **Tradeoffs:**

1\. Robustness vs Correctness (from Code Complete by Steve McConnell):

If a down stream service is broken, it's better to return cached data than to show an error message (ie. prioritise robustness over correctness).

The retry mechanism and the circuit breaker pattern can be used together in Polly to provide robustness	and system stability.



2\. First Contentful Paint (FCP) and Total Load Time:

There are two options:

&nbsp;a) One HTTP request to get all the shelves, but this takes longer to return data and means the user can't see anything above the fold while it's running.

&nbsp;b) Two HTTP requests to get the critical (non-personalised) shelves first, then the personalised shelves.

&nbsp;   The overall load time is longer but at least the user sees something above the fold quickly.

&nbsp;We will go with option b).



3\. Parallelisation:

API calls are being made in parallel. This means the current user is able to get a response more quickly, but at the expense of there being fewer threads available in the thread pool to service incoming requests.
We have prioritised the current user's experience.



4\. Caching (not yet implemented):
We will be able to show content very quickly at the expense of it possibly being not the most up to date content.
This is acceptable for a streaming app like this, but obviously unacceptable for other industries like banking or medicine.



#### **What I would add if I had more time:**

**Caching:**

Cache results for non-personalized shelves to improve load times and resilience.

This is the **most important** concept for this project. There are many things that can be cached and a streaming app is the perfect type of app to use caching.

IMemoryCache would be the simplest way and doesn't require serialisation, however in a real world scenario, there are many instances of the web service running behind a load balancer.

A Redis cache would need to be used to share the data across instances.



**Error Handling \& Resilience:**

Implement robust error handling for downstream service failures (eg, fallback to cached data, partial results, or default shelves).

Consider using Polly for retries and circuit breakers on HTTP calls.



**Personalized Shelves:**

Implement the async loading and separation of personalized shelves (ran out of time to do this).



**Unit \& Integration Tests:**

Add tests for service logic, especially for shelf composition and error scenarios.



**Logging \& Monitoring:**

Add logging for service calls and failures to aid in debugging and production monitoring.



**Validation:**

Validate incoming data from layout and downstream services to avoid runtime errors.



**Code:**
Be more consistent with collection types. IList<T> should be used for public methods and List<T> for private methods.

Look at updating the /shows/api/shows/:showId endpoint to accept a list of showIds so we can get back multiple shows in the one request.

Look at why the deserialisation wasn't working when converting the JSON string from the layout service to a concrete object.

