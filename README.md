# SiteAvailabilityChecker
This is a single site availability checker. It checks if site responde to a HEAD request, and if the IP matches the expected IP
I use this to verify that the site is running and that the IP going to my house has not changes (because I use dynamic DNS).

The solution consists of a windows service doing the work of checking the status, and a program showing a tray icon.
The icon will be green if all is well. Otherwise red (or yellow if service was restarted).

The service will check if site is available and that its ip matches the expected.


