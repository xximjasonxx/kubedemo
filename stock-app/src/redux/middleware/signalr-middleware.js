import { START_CONNECTION } from "../actions/stockprice-actions";

export function signalr_middleware(store) {
    return (next) => async (action) => {
        switch (action.type) {
            case START_CONNECTION:
        }
    }
}