
import React from 'react';
import { HubConnectionBuilder, LogLevel } from '@aspnet/signalr';

class MainLanding extends React.Component {
    constructor(props) {
        super(props);

        this.state = {
            hubConnection: null,
            stockPrices: [],
            stockSymbols: []
        };

        this.handleConnectionStart = this.handleConnectionStart.bind(this);
    }

    componentDidMount = () => {
        const hubConnection = new HubConnectionBuilder()
            .withUrl("http://192.168.99.100:31000/prices")
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
            var dataObject = {
                symbol: stockPrice.symbol,
                price: stockPrice.newPrice,
                publishTime: stockPrice.publishTime };

            var symbols = this.state.stockSymbols;
            if (symbols.filter(symbol => symbol === stockPrice.symbol).length === 0) {
                symbols = [ ...symbols, stockPrice.symbol ];
            }

            this.setState({
                stockPrices: [ ...this.state.stockPrices, dataObject ],
                stockSymbols: symbols
            });
        });
    }

    render() {
        const { stockPrices, stockSymbols } = this.state;

        return (
            <div>
                <h2>Stock Prices</h2>
                <hr />
                {stockSymbols.map(symbol => {
                    return (
                        <h3 key={symbol}>{symbol}</h3>
                    );
                })}
            </div>
        );
    }
};

export default MainLanding;
