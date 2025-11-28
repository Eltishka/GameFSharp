module Event

type EventState = 
| None
| Evoked
| Skipped

type Event = 
    Name: string
    State: EventState
    Condition: Map<string, RectShape -> bool
    Action: Map<string, RectShape -> unit

type EventLoop = 
    Events: Event list

