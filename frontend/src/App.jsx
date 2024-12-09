import { Route, Routes } from 'react-router-dom';
import Gatewayspage from './pages/Gatewayspage/Gatewayspage';
import MainLayout from './layouts/MainLayout';
import Gatewaypage from './pages/Gatewaypage/Gatewaypage';
import Assignmentspage from './pages/Assignmentspage/Assignmentspage';
import Form from './components/UI/Form/Form';
import Rulespage from './pages/Rulespage/Rulespage';
import Testingpage from './pages/Testingpage/Testingpage';
import { ROUTES } from './data/routes';
import { NotFoundpage } from './pages/NotFoundpage/NotFoundpage';
import { AssignmentEditpage } from './pages/AssignmentEditpage/AssignmentEditpage';
import { AssignmentCreatepage } from './pages/AssignmentCreatepage/AssignmentCreatepage';

function App() {
    return (
        <>
            <Routes>
                <Route path="/" element={<MainLayout />}>
                    <Route index element={<Gatewayspage />} />
                    <Route
                        path={`${ROUTES.GATEWAYS}/:id`}
                        element={<Gatewaypage />}
                    />
                    <Route
                        path={`${ROUTES.GATEWAYS}/create`}
                        element={
                            <Form types={[{ name: 'IP', input: 'input' }]} />
                        }
                    />
                    <Route
                        path={ROUTES.ASSIGNMENTS}
                        element={<Assignmentspage />}
                    />

                    <Route
                        path={`${ROUTES.ASSIGNMENTS}/create`}
                        element={<AssignmentCreatepage />}
                    ></Route>
                    <Route
                        path={`${ROUTES.ASSIGNMENTS}/edit`}
                        element={<AssignmentEditpage />}
                    ></Route>
                    <Route path={`${ROUTES.RULES}`} element={<Rulespage />} />
                    <Route
                        path={`${ROUTES.RULES}/edit`}
                        element={
                            <Form
                                types={[
                                    { name: 'Name', input: 'input' },
                                    { name: 'Condition', input: 'input' },
                                    { name: 'Assigment?', input: 'select' },
                                ]}
                            />
                        }
                    />
                    <Route
                        path={`${ROUTES.RULES}/create`}
                        element={
                            <Form
                                types={[
                                    { name: 'Name', input: 'input' },
                                    { name: 'Condition', input: 'input' },
                                    { name: 'Assigment?', input: 'select' },
                                ]}
                            />
                        }
                    />
                    <Route path={ROUTES.TESTING} element={<Testingpage />} />
                    <Route path="*" element={<NotFoundpage />} />
                </Route>
            </Routes>
        </>
    );
}

export default App;
