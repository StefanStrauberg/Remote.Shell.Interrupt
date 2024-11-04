import { Link, useLocation } from 'react-router-dom';
import classes from './Header.module.css';
import Button from '../UI/Button/Button';
import { ROUTES } from '../../data/routes';
import Input from '../UI/Input/Input';
import { useDispatch, useSelector } from 'react-redux';
import { GATEWAY_SEARCH_ADDRESS_ACTIONS } from '../../store/gatewaySearchAddressReducer';
import { useDebounce } from '../../hooks/useDebounce';
import { useEffect, useState } from 'react';

export default function Header() {
    const location = useLocation();
    const dispatch = useDispatch();
    const [searchValue, setSearchValue] = useState();
    const searchValueDebounced = useDebounce(searchValue);
    const gatewayAddress = useSelector(
        (state) => state.gatewaySearchAddressReducer
    );

    const onChangeSearch = (value) => {
        setSearchValue(value);
    };
    useEffect(() => {
        dispatch({
            type: GATEWAY_SEARCH_ADDRESS_ACTIONS.CHANGE_VALUE,
            payload: searchValueDebounced,
        });
    }, [searchValueDebounced]);
    useEffect(() => {
        setSearchValue('');
    }, [location.pathname]);

    return (
        <header className={classes.header}>
            <div className={classes.pages}>
                <Link to={'/'}>
                    <div
                        className={`${classes.page} ${
                            location.pathname
                                .toLowerCase()
                                .includes('gateways') ||
                            location.pathname === '/'
                                ? classes.activePage
                                : ''
                        }`}
                    >
                        Gateways
                    </div>
                </Link>
                <Link to={ROUTES.ASSIGNMENTS}>
                    <div
                        className={`${classes.page} ${
                            location.pathname
                                .toLowerCase()
                                .includes('assignments')
                                ? classes.activePage
                                : ''
                        } `}
                    >
                        Assigments
                    </div>
                </Link>
                <Link to={ROUTES.RULES}>
                    <div
                        className={`${classes.page} ${
                            location.pathname.toLowerCase().includes('rules')
                                ? classes.activePage
                                : ''
                        } `}
                    >
                        Rules
                    </div>
                </Link>
                <Link
                    to={ROUTES.TESTING}
                    className={`${classes.page} ${
                        location.pathname.toLowerCase().includes('testing')
                            ? classes.activePage
                            : ''
                    } `}
                >
                    Testing
                </Link>
            </div>
            {location.pathname === '/' ? (
                <div className={classes.search}>
                    <Input
                        placeholder={'Поиск gateway по IP'}
                        onChange={(e) => onChangeSearch(e.target.value)}
                        defaultValue={gatewayAddress.value}
                    />
                </div>
            ) : null}

            {
                <div
                    className={classes.btns}
                    style={{
                        pointerEvents: location.pathname.includes('/testing')
                            ? 'none'
                            : 'auto',
                        opacity: location.pathname.includes('/testing') ? 0 : 1,
                    }}
                >
                    <Link
                        to={
                            location.pathname.includes('create')
                                ? location.pathname
                                : location.pathname === '/' ||
                                  location.pathname.includes('gateways')
                                ? 'gateways/create'
                                : location.pathname.includes('assignments')
                                ? 'assignments/create'
                                : 'rules/create'
                        }
                    >
                        <Button>Create new</Button>
                    </Link>
                </div>
            }
        </header>
    );
}
