
import React from 'react';
import { HubConnectionBuilder, LogLevel } from '@aspnet/signalr';
import config from '../config';
import StockListing from './StockListing';

class MainLanding extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            hubConnection: null,
            stockPrices: {}
        };

        this.handleConnectionStart = this.handleConnectionStart.bind(this);
    }

    componentDidMount = () => {
        const hubConnection = new HubConnectionBuilder()
            .withUrl(`${config.API_HOSTNAME}/prices`)
            .configureLogging(LogLevel.Information)
            .build();

        this.setState({ hubConnection }, () => {
            this.state.hubConnection
                .start()
                .then(this.handleConnectionStart)
                .catch(err => {
                    console.log('Connection error: ' + err);
                });
        });
    }

    handleConnectionStart() {
        console.log("Connection established");
        this.state.hubConnection.on('ReceiveStockPrice', (stockPrice) => {    
            var newState = Object.assign(this.state.stockPrices);
            newState[stockPrice.symbol] = stockPrice;
            
            this.setState(newState);
        });
    }

    render() {
        const { stockPrices } = this.state;
        return (
            <div style={{ marginLeft: '20px' }}>
                <h2>Stock Prices</h2>
                <hr />
                {Object.keys(stockPrices).map((key) => {
                    return <StockListing key={key} stockPrice={stockPrices[key]} />;
                })}
            </div>
        );
    }
};

export default MainLanding;
