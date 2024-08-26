import { Link, useLocation } from 'react-router-dom';
import classes from './Header.module.css';
import Input from '../UI/Input/Input';
import Button from '../UI/Button/Button';

export default function Header() {
    const location = useLocation();
    console.log(location);
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
                <Link to={'/assignments'}>
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
                <Link to={'/rules'}>
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
                    to={'/testing'}
                    className={`${classes.page} ${
                        location.pathname.toLowerCase().includes('testing')
                            ? classes.activePage
                            : ''
                    } `}
                >
                    Testing
                </Link>
            </div>
            {/*             <div className={classes.search}>
                <Input placeholder={'Поиск информации по сайту'} />
                <Button>Найти</Button>
            </div> */}
            {location.pathname !== '/testing' && (
                <div className={classes.btns}>
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
            )}
        </header>
    );
}
